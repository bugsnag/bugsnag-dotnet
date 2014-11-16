using Bugsnag.Core.Payload;
using System;
using System.Diagnostics;
using System.Linq;

namespace Bugsnag.Core
{
    public class Event
    {
        public Exception Exception { get; private set; }
        public bool? IsRuntimeEnding { get; private set; }
        public StackTrace CallTrace { get; set; }

        public string GroupingHash { get; set; }
        public Severity Severity { get; set; }
        public Metadata Metadata { get; private set; }

        private const string CallTraceHeading = "[NOTIFY CALL STACK (stack trace not available)]";

        public Event(Exception exception) : this(exception, null) { }

        public Event(Exception exception, bool? runtimeEnding)
        {
            Exception = exception;
            IsRuntimeEnding = runtimeEnding;
            Severity = Severity.Error;
            Metadata = new Metadata();

            // Record a full notify stack trace if the exception has none (ignoring the first constructor stack frame)
            if (Exception.StackTrace == null)
                CallTrace = new StackTrace(1, true);
        }

        public ExceptionInfo GenerateExceptionInfo(Configuration config)
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
                Message = Exception.Message + (usedCallTrace ? " " + CallTraceHeading : ""),
                StackTrace = stackFrameInfos
            };
        }

        private static StackTraceFrameInfo ExtractStackTraceFrameInfo(StackFrame frame, Configuration config)
        {
            var method = frame.GetMethod();

            var param = method.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name);
            var paramSummary = String.Join(",", param);
            var signature = String.Format("{0}({1})", method.Name, paramSummary);

            var methodInfo = method.DeclaringType == null ? "" : method.DeclaringType.FullName;
            methodInfo += "." + signature;

            var file = frame.GetFileName();
            if (config.FilePrefix != null && !String.IsNullOrEmpty(file))
            {
                config.FilePrefix.ToList().ForEach(x => file = file.Replace(x, ""));
            }

            var inProject = true;
            if (config.AutoDetectInProject)
            {
                inProject = !String.IsNullOrEmpty(file);
            }
            else
            {
                if (config.ProjectNamespaces != null && method.DeclaringType != null)
                    inProject = config.ProjectNamespaces.Any(x => method.DeclaringType.FullName.StartsWith(x, StringComparison.Ordinal));
            }

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
