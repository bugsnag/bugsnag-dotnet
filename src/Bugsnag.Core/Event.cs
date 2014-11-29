using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Bugsnag.Core.Payload;

namespace Bugsnag.Core
{
    public class Event
    {
        public Exception Exception { get; private set; }
        public bool IsRuntimeEnding { get; private set; }
        public StackTrace CallTrace { get; private set; }

        public string GroupingHash { get; set; }
        public Severity Severity { get; set; }
        public Metadata Metadata { get; set; }

        public Event(Exception exception)
            : this(exception, false)
        {
        }

        public Event(Exception exception, bool runtimeEnding)
        {
            Exception = exception;
            IsRuntimeEnding = runtimeEnding;
            Severity = Severity.Error;
            Metadata = new Metadata();

            // Record a full notify stack trace if the exception has none (ignoring the first constructor stack frame)
            if (Exception == null || Exception.StackTrace == null)
                CallTrace = new StackTrace(1, true);

        }

        public ExceptionInfo GenerateExceptionInfo(IConfiguration config)
        {
            bool usedCallTrace = false;
            var trace = new StackTrace(Exception, true);
            if (trace == null || trace.FrameCount == 0)
            {
                trace = CallTrace;
                usedCallTrace = true;
            }

            if (trace == null)
                return null;

            var frames = trace.GetFrames();
            if (frames == null)
                return null;

            var stackFrameInfos = frames.Select(x => ExtractStackTraceFrameInfo(x, config)).ToList();
            return new ExceptionInfo
            {
                ExceptionClass = Exception.GetType().Name,
                Description = Exception.Message + (usedCallTrace ? " [CALL STACK]" : ""),
                StackTrace = stackFrameInfos
            };
        }

        private static StackTraceFrameInfo ExtractStackTraceFrameInfo(StackFrame frame, IConfiguration config)
        {
            var method = frame.GetMethod();

            var param = method.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name);
            var paramSummary = String.Join(",", param);
            var signature = String.Format(CultureInfo.CurrentCulture, "{0}({1})", method.Name, paramSummary);

            var methodInfo = method.DeclaringType == null ? "" : method.DeclaringType.FullName;
            methodInfo += "." + signature;

            var file = config.RemoveFileNamePrefix(frame.GetFileName());
            var inProject = (config.AutoDetectInProject && !String.IsNullOrEmpty(file)) ||
                            config.IsInProjectNamespace(method.DeclaringType.FullName);

            return new StackTraceFrameInfo
            {
                File = file,
                LineNumber = frame.GetFileLineNumber(),
                Method = methodInfo,
                InProject = inProject
            };
        }
    }
}
