using Bugsnag.Event;
using Bugsnag.Message;
using Bugsnag.Message.App;
using Bugsnag.Message.Core;
using Bugsnag.Message.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var notification = new Notification
            {
                ApiKey = config.ApiKey,
                Notifier = Notifier.NotifierInfo,
                Events = new List<EventInfo>()
            };
            return notification;
        }

        private static EventInfo CreateBase(Bugsnag.Event.Event eventData, Configuration config)
        {
            var appInfo = new AppInfo
            {
                Version = config.AppVersion,
                ReleaseStage = config.ReleaseStage
            };

            var userInfo = new UserInfo
            {
                Id = config.UserId,
                Email = config.UserEmail,
                Name = config.UserName
            };

            var eventInfo = new EventInfo
            {
                App = appInfo,
                Device = Profiler.DeviceInfo,
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
                List<StackTraceFrameInfo> frames = null;
                if (stackFrames != null)
                    frames = stackFrames.Select(x => ExtractFrameInfo(x, config.TrimFilenames)).ToList();

                var expInfo = new ExceptionInfo
                {
                    ExceptionClass = currentExp.GetType().Name,
                    Message = currentExp.Message,
                    StackTrace = frames
                };

                expInfos.Add(expInfo);
                currentExp = currentExp.InnerException;
            }

            return expInfos;
        }

        private static StackTraceFrameInfo ExtractFrameInfo(StackFrame frame, bool trimFilename = true)
        {
            var method = frame.GetMethod();

            var param = method.GetParameters()
                  .Select(p => String.Format("{0} {1}", p.ParameterType.Name, p.Name))
                  .ToArray();

            string signature = String.Format("{0}({1})", method.Name, String.Join(",", param));

            var methodInfo = method.DeclaringType == null ? "" : method.DeclaringType.FullName;
            methodInfo += "." + signature;

            var file = frame.GetFileName();
            if (!String.IsNullOrEmpty(file))
                file = file.Replace(@"e:\GitHub\Bugsnag-NET\BugsnagDemoMVC\", "");

            return new StackTraceFrameInfo
            {
                File = file,
                LineNumber = frame.GetFileLineNumber(),
                Method = methodInfo,
                InProject = !String.IsNullOrEmpty(file)
            };
        }
    }
}
