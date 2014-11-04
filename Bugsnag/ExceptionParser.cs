using Bugsnag.Message.Event;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bugsnag
{
    public class ExceptionParser
    {
        private const string CallTraceHeading = "[NOTIFY CALL STACK (stack trace not available)]";

        private Configuration Config { get; set; }

        public ExceptionParser(Configuration config)
        {
            Config = config;
        }

        public ExceptionInfo ExtractExceptionInfo(Exception exp, StackTrace callTrace = null)
        {
            bool usedCallTrace = false;
            var trace = new StackTrace(exp, true);
            if (trace == null)
            {
                trace = callTrace;
                usedCallTrace = true;
            }

            if (trace == null)
                throw new ArgumentException("No valid stack trace in exception or no valid call stack trace was provided");

            var frames = trace.GetFrames();
            if (frames == null)
                throw new ArgumentException("Unable to extract frames from stack trace");

            var stackFrameInfos = frames.Select(ExtractStackTraceFrameInfo).ToList();

            return new ExceptionInfo
                       {
                           ExceptionClass = exp.GetType().Name,
                           Message = exp.Message + (usedCallTrace ? " " + CallTraceHeading : ""),
                           StackTrace = stackFrameInfos
                       };
        }

        public StackTraceFrameInfo ExtractStackTraceFrameInfo(StackFrame frame)
        {
            var method = frame.GetMethod();

            var param = method.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name);
            var paramSummary = String.Join(",", param);
            var signature = String.Format("{0}({1})", method.Name, paramSummary);

            var methodInfo = method.DeclaringType == null ? "" : method.DeclaringType.FullName;
            methodInfo += "." + signature;

            var file = frame.GetFileName();
            if (Config.FilePrefix != null && !String.IsNullOrEmpty(file))
            {
                Config.FilePrefix.ToList().ForEach(x => file = file.Replace(x, ""));
            }

            var inProject = true;
            if (Config.AutoDetectInProject)
            {
                inProject = !String.IsNullOrEmpty(file);
            }
            else
            {
                if (Config.ProjectNamespaces != null && method.DeclaringType != null)
                    inProject = Config.ProjectNamespaces.Any(x => method.DeclaringType.FullName.StartsWith(x));
            }
            
            return new StackTraceFrameInfo
            {
                File = file,
                LineNumber = frame.GetFileLineNumber(),
                Method = methodInfo,
                InProject = inProject
            };
        }

        public List<ThreadInfo> CreateThreadsInfo()
        {
            //TODO : WORK IN PROGRESS
            return null;

            var threadInfos = new List<ThreadInfo>();
            var result = new Dictionary<int, string[]>();

            var pid = Process.GetCurrentProcess().Id;

            using (var dataTarget = DataTarget.AttachToProcess(pid, 5000, AttachFlag.Passive))
            {
                string dacLocation = dataTarget.ClrVersions[0].TryGetDacLocation();
                var runtime = dataTarget.CreateRuntime(dacLocation);

                foreach (var t in runtime.Threads)
                {
                    result.Add(
                        t.ManagedThreadId,
                        t.StackTrace.Select(f =>
                        {
                            if (f.Method != null)
                            {
                                return f.Method.Type.Name + "." + f.Method.Name;
                            }

                            return null;
                        }).ToArray()
                    );
                }
            }

            return threadInfos;
        }

    }
}
