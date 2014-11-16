using Bugsnag.Core.Payload;
using Bugsnag.Core.Payload.App;
using Bugsnag.Core.Payload.Device;
using Bugsnag.Core.Payload.Event;
using System.Collections.Generic;

namespace Bugsnag.Core
{
    public class NotificationFactory
    {
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

        private EventInfo CreateEventInfoBase(Event errorData)
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
                Severity = errorData.Severity,
                User = userInfo,
                Context = Config.Context,
                GroupingHash = errorData.GroupingHash
            };
            return eventInfo;
        }

        private EventInfo CreateErrorEventInfo(Event error)
        {
            var errInfo = CreateEventInfoBase(error);
            var exps = CreateExceptionsInfo(error);
            if (exps == null)
                return null;

            errInfo.Exceptions = exps;

            // TODO Find a way to snapshot all manage threads at this point
            //if (Config.SendThreads)
            //    errInfo.Threads = CreateThreadsInfo(Config);

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

        private List<ExceptionInfo> CreateExceptionsInfo(Event error)
        {
            // Create a list of exception information
            var expInfos = new List<ExceptionInfo>();

            // Keep track of the current exception your trying to add. As we add an exception, we will
            // move to the inner exception and so on, until all the exceptions in a nested exception is recorded
            var currentExp = error.Exception;
            while (currentExp != null)
            {
                var expInfo = error.GenerateExceptionInfo(Config);
                if (expInfo != null)
                    expInfos.Add(expInfo);
                currentExp = currentExp.InnerException;
            }
            return expInfos.Count == 0 ? null : expInfos;
        }
    }
}
