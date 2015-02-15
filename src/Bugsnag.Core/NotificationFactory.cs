using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bugsnag.Payload;

namespace Bugsnag
{
    internal class NotificationFactory
    {
        private const string ExpDetailsTabName = "Exception Details";

        private Configuration Config { get; set; }

        public NotificationFactory(Configuration config)
        {
            Config = config;
        }

        public Notification CreateFromError(Event error)
        {
            var notification = CreateNotificationBase();
            var eventInfo = CreateErrorEventInfo(error);
            if (eventInfo == null)
                return null;

            notification.Events.AddRange(eventInfo);
            return notification;
        }

        private Notification CreateNotificationBase()
        {
            var notifierInfo = new NotifierInfo
            {
                Name = Notifier.Name,
                Version = Notifier.Version,
                Url = Notifier.Url.AbsoluteUri
            };

            var notification = new Notification
            {
                ApiKey = Config.ApiKey,
                Notifier = notifierInfo,
                Events = new List<EventInfo>()
            };
            return notification;
        }

        private EventInfo CreateEventInfoBase(Event errorData)
        {
            var appInfo = new AppInfo
            {
                Version = Config.AppVersion,
                ReleaseStage = Config.ReleaseStage,
                AppArchitecture = Diagnostics.AppArchitecture,
                ClrVersion = Diagnostics.ClrVersion
            };

            var userInfo = new UserInfo
            {
                Id = errorData.UserId,
                Email = errorData.UserEmail,
                Name = errorData.UserName
            };

            var deviceInfo = new DeviceInfo
            {
                OSVersion = Diagnostics.DetectedOSVersion,
                ServicePack = Diagnostics.ServicePack,
                OSArchitecture = Diagnostics.OSArchitecture,
                ProcessorCount = Diagnostics.ProcessorCount,
                MachineName = Diagnostics.MachineName,
                HostName = Diagnostics.HostName
            };

            var eventInfo = new EventInfo
            {
                App = appInfo,
                Device = deviceInfo,
                Severity = errorData.Severity,
                User = userInfo,
                Context = errorData.Context,
                GroupingHash = errorData.GroupingHash,
                Exceptions = new List<ExceptionInfo>()
            };
            return eventInfo;
        }

        private List<EventInfo> CreateErrorEventInfo(Event error)
        {
            var baseEvent = CreateEventInfoBase(error);
            var allEvents = new List<EventInfo> { baseEvent };
            RecursiveAddExceptionInfo(error, error.Exception, error.CallTrace, allEvents, baseEvent);
            return allEvents;
        }

        /// <summary>
        /// Starting with an exception, will recursively add exception infos to event infos.
        /// If an aggregate exception is encountered, an event info will be created for every inner exception
        /// to ensure no information is lost.
        /// </summary>
        /// <param name="error">The error to create new event infos</param>
        /// <param name="exp">The exception to add to the current event</param>
        /// <param name="callStack">The stack of notify call</param>
        /// <param name="allEventInfos">The current status of all event infos created</param>
        /// <param name="currentEvent">The current event info to add the exception to</param>
        private void RecursiveAddExceptionInfo(Event error,
                                               Exception exp,
                                               StackTrace callStack,
                                               List<EventInfo> allEventInfos,
                                               EventInfo currentEvent)
        {
            // If we have no more exceptions, return the generated event infos
            if (exp == null) return;

            // Parse the exception and add it to the current event info stack
            var expInfo = ExceptionParser.GenerateExceptionInfo(exp, callStack, Config);
            if (expInfo != null)
                currentEvent.Exceptions.Add(expInfo);

            // If the exception has no inner exception, then we are finished. Generate metadata for the last exception
            // and set the final metadata for the event info
            if (exp.InnerException == null)
            {
                FinaliseEventInfo(currentEvent, error, exp);
            }
            else
            {
                // Check if the current exception contains more than 1 inner exception
                // if it does, then clone new events for every inner exception and recurse through them seperately
                var aggExp = exp as AggregateException;
                if (aggExp != null && aggExp.InnerExceptions.Count > 1)
                {
                    allEventInfos.Remove(currentEvent);
                    foreach (var inner in aggExp.InnerExceptions)
                    {
                        var newEvent = CreateEventInfoBase(error);
                        newEvent.Exceptions = new List<ExceptionInfo>(currentEvent.Exceptions);
                        allEventInfos.Add(newEvent);
                        RecursiveAddExceptionInfo(error, inner, callStack, allEventInfos, newEvent);
                    }
                }
                else
                {
                    // Otherwise just move to the next inner exception
                    RecursiveAddExceptionInfo(error, exp.InnerException, callStack, allEventInfos, currentEvent);
                }
            }
        }

        /// <summary>
        /// Finalises the event info by adding final metadata 
        /// </summary>
        /// <param name="eventInfo">The event info to finalise</param>
        /// <param name="error">The responsible event</param>
        /// <param name="lastException">The root exception of the event info</param>
        private void FinaliseEventInfo(EventInfo eventInfo, Event error, Exception lastException)
        {
            var expMetaData = new Metadata();

            expMetaData.AddToTab(ExpDetailsTabName, "runtimeEnding", error.IsRuntimeEnding);

            if (lastException.HelpLink != null)
                expMetaData.AddToTab(ExpDetailsTabName, "helpLink", lastException.HelpLink);

            if (lastException.Source != null)
                expMetaData.AddToTab(ExpDetailsTabName, "source", lastException.Source);

            if (lastException.TargetSite != null)
                expMetaData.AddToTab(ExpDetailsTabName, "targetSite", lastException.TargetSite);

            var metaData = Metadata.CombineMetadata(Config.Metadata, error.Metadata, expMetaData);
            metaData.FilterEntries(Config.IsEntryFiltered);
            eventInfo.Metadata = metaData.MetadataStore;
        }
    }
}
