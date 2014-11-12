using Bugsnag.Event;
using Bugsnag.Payload;
using Bugsnag.Payload.App;
using Bugsnag.Payload.Core;
using Bugsnag.Payload.Device;
using Bugsnag.Payload.Event;
using System.Collections.Generic;
using BugsnagEvent = Bugsnag.Event.Event;

namespace Bugsnag
{
    public class NotificationFactory
    {
        private Configuration Config { get; set; }
        private ExceptionParser ExpParser { get; set; }

        public NotificationFactory(Configuration config)
        {
            Config = config;
            ExpParser = new ExceptionParser(config);
        }

        public Notification CreateFromError(Error error)
        {
            var notification = CreateNotificationBase();
            var eventInfo = CreateErrorEventInfo(error);
            if (eventInfo == null)
                return null;

            notification.Events.Add(eventInfo);
            return notification;
        }

        private Notification CreateNotificationBase()
        {
            var notifierInfo = new NotifierInfo
            {
                Name = Notifier.Name,
                Version = Notifier.Version,
                Url = Notifier.Url
            };

            var notification = new Notification
            {
                ApiKey = Config.ApiKey,
                Notifier = notifierInfo,
                Events = new List<EventInfo>()
            };
            return notification;
        }

        private EventInfo CreateEventInfoBase(BugsnagEvent eventData)
        {
            var appInfo = new AppInfo
            {
                Version = Config.AppVersion,
                ReleaseStage = Config.ReleaseStage,
                AppArchitecture = Profiler.AppArchitecture,
                ClrVersion = Profiler.ClrVersion
            };

            var userInfo = new UserInfo
            {
                Id = Config.UserId,
                Email = Config.UserEmail,
                Name = Config.UserName,
                LoggedOnUser = Config.LoggedOnUser
            };

            var deviceInfo = new DeviceInfo
            {
                OsVersion = Profiler.DetectedOsVersion,
                ServicePack = Profiler.ServicePack,
                OsArchitecture = Profiler.OsArchitecture,
                ProcessorCount = Profiler.ProcessorCount,
                MachineName = Profiler.MachineName
            };

            var eventInfo = new EventInfo
            {
                App = appInfo,
                Device = deviceInfo,
                Severity = eventData.Severity,
                User = userInfo,
                Context = Config.Context,
                GroupingHash = eventData.GroupingHash
            };
            return eventInfo;
        }

        private EventInfo CreateErrorEventInfo(Error error)
        {
            var errInfo = CreateEventInfoBase(error);
            var exps = CreateExceptionsInfo(error);
            if (exps == null)
                return null;

            errInfo.Exceptions = exps;
            if (Config.SendThreads)
                errInfo.Threads = ExpParser.CreateThreadsInfo();
            
            // Get to the inner most exception
            var innerExp = error.Exception;
            while (innerExp.InnerException != null)
                innerExp = innerExp.InnerException;

            var expMetaData = new MetaData();
            const string expDetailsTabName = "Exception Details";

            // Record the exception details if there are any
            if (error.IsRuntimeEnding != null)
                expMetaData.AddToTab(expDetailsTabName, "runtimeEnding", error.IsRuntimeEnding);

            if (innerExp.HelpLink != null)
                expMetaData.AddToTab(expDetailsTabName, "helpLink", innerExp.HelpLink);

            if (innerExp.Source != null)
                expMetaData.AddToTab(expDetailsTabName, "source", innerExp.Source);

            if (innerExp.TargetSite != null)
                expMetaData.AddToTab(expDetailsTabName, "targetSite", innerExp.TargetSite);

            errInfo.MetaData = MetaData.MergeMetaData(Config.StaticData, error.MetaData, expMetaData).MetaDataStore;

            return errInfo;
        }

        private List<ExceptionInfo> CreateExceptionsInfo(Error error)
        {
            // Create a list of exception information
            var expInfos = new List<ExceptionInfo>();

            // Keep track of the current exception your trying to add. As we add an exception, we will
            // move to the inner exception and so on, until all the exceptions in a nested exception is recorded
            var currentExp = error.Exception;
            while (currentExp != null)
            {
                var expInfo = ExpParser.ExtractExceptionInfo(currentExp, error.CallTrace);
                if (expInfo != null)
                    expInfos.Add(expInfo);
                currentExp = currentExp.InnerException;
            }
            return expInfos.Count == 0 ? null : expInfos;
        }
    }
}
