using Bugsnag.Event;
using Bugsnag.Message;
using Bugsnag.Message.App;
using Bugsnag.Message.Core;
using Bugsnag.Message.Device;
using Bugsnag.Message.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bugsnag
{
    public static class NotificationFactory
    {
        public static Notification CreateFromError(Error error, Configuration config)
        {
            var notification = CreateBase(config);
            var eventInfo = CreateEventInfo(error, config);
            notification.Events.Add(eventInfo);
            return notification;
        }

        private static Notification CreateBase(Configuration config)
        {
            var notifierInfo = new NotifierInfo
            {
                Name = Notifier.Name,
                Version = Notifier.Version,
                Url = Notifier.Url
            };

            var notification = new Notification
            {
                ApiKey = config.ApiKey,
                Notifier = notifierInfo,
                Events = new List<EventInfo>()
            };
            return notification;
        }

        private static EventInfo CreateBase(Bugsnag.Event.Event eventData, Configuration config)
        {
            var appInfo = new AppInfo
            {
                Version = config.AppVersion,
                ReleaseStage = config.ReleaseStage,
                AppArchitecture = Profiler.AppArchitecture,
                ClrVersion = Profiler.ClrVersion
            };

            var userInfo = new UserInfo
            {
                Id = config.UserId,
                Email = config.UserEmail,
                Name = config.UserName,
                LoggedOnUser = config.LoggedOnUser
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
                Context = eventData.Context,
                GroupingHash = eventData.GroupingHash
            };
            return eventInfo;
        }

        private static EventInfo CreateEventInfo(Error error, Configuration config)
        {
            var errInfo = CreateBase(error, config);
            errInfo.Exceptions = CreateExceptionInfo(error, config);
            errInfo.MetaData = MetaData.GenerateMetaDataOutput(config.StaticData, error.MetaData);
            return errInfo;
        }

        private static List<ExceptionInfo> CreateExceptionInfo(Error error, Configuration config)
        {
            // Create a list of exception information
            var expInfos = new List<ExceptionInfo>();

            // Keep track of the current exception your trying to add. As we add an exception, we will
            // move to the inner exception and so on, until all the exceptions in a nested exception is recorded
            var currentExp = error.Exception;
            while (currentExp != null)
            {
                var stackFrames = new StackTrace(currentExp, true).GetFrames();
                List<StackTraceFrameInfo> frames = new List<StackTraceFrameInfo>();
                if (stackFrames != null)
                    frames = stackFrames.Select(x => ExtractFrameInfo(x, config.FilePrefix, config.AutoDetectInProject)).ToList();
                else if (error.CallTrace != null)
                    frames = error.CallTrace.GetFrames().Skip(1).Select(x => ExtractFrameInfo(x, config.FilePrefix, config.AutoDetectInProject)).ToList();

                var expInfo = new ExceptionInfo
                {
                    ExceptionClass = currentExp.GetType().Name,
                    Message = currentExp.Message + (error.CallTrace != null ? " [NOTIFY CALL STACK (stack trace not available)]" : ""),
                    StackTrace = frames
                };

                expInfos.Add(expInfo);
                currentExp = currentExp.InnerException;
            }

            return expInfos;
        }

        private static StackTraceFrameInfo ExtractFrameInfo(StackFrame frame, string[] filePrefix, bool autoDetectInProject)
        {
            var method = frame.GetMethod();

            var param = method.GetParameters()
                  .Select(p => String.Format("{0} {1}", p.ParameterType.Name, p.Name))
                  .ToArray();

            string signature = String.Format("{0}({1})", method.Name, String.Join(",", param));

            var methodInfo = method.DeclaringType == null ? "" : method.DeclaringType.FullName;
            methodInfo += "." + signature;

            var file = frame.GetFileName();
            if (filePrefix != null && !String.IsNullOrEmpty(file))
            {
                foreach(var prefix in filePrefix)
                {
                    file = file.Replace(prefix, "");
                }
            }

            return new StackTraceFrameInfo
            {
                File = file,
                LineNumber = frame.GetFileLineNumber(),
                Method = methodInfo,
                InProject = autoDetectInProject ? !String.IsNullOrEmpty(file) : true
            };
        }
    }
}
