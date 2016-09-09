using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Bugsnag.Payload;

namespace Bugsnag
{
    /// <summary>
    /// Defines helper methods used to convert exception information into payload data
    /// </summary>
    internal static class ExceptionParser
    {
        /// <summary>
        /// Creates a exception information payload based on an exception
        /// </summary>
        /// <param name="exception">The exception to base the payload on</param>
        /// <param name="callTrace">The call stack trace, or null if its not available</param>
        /// <param name="lastTrace">The last stack trace of the exception one level up, or null if its not available.</param>
        /// <param name="config">The configuration to use</param>
        /// <param name="expTrace">The trace used when generating the exception info, or null if one was not used</param>
        /// <returns>The exception information payload to use in the notification</returns>
        public static ExceptionInfo GenerateExceptionInfo(Exception exception, 
                                                          StackTrace callTrace, 
                                                          StackTrace lastTrace, 
                                                          Configuration config, 
                                                          out StackTrace expTrace)
        {
            expTrace = null;
            if (exception == null)
                return null;

            StackTrace trace = null;

            // Attempt to get stack frame from exception, if the stack trace is invalid,
            // try to use the previous exception stack trace or call stack trace
            trace = new StackTrace(exception, true);
            if (trace == null || trace.FrameCount == 0)
            {
                // Try to use the last trace for an outer exception. If thats not available, use
                // the call trace and if thats not available, then give up and return null as we
                // have no useable stacktrace.
                if (lastTrace != null && lastTrace.FrameCount != 0)
                    trace = lastTrace;
                else if (callTrace != null && callTrace.FrameCount != 0)
                    trace = callTrace;
                else
                    return null;
            }

            // Output the trace used
            expTrace = trace;

            // Attempt to get the stack frames
            var frames = trace.GetFrames();
            if (frames == null)
                return null;

            // Convert the frames to stack frame payloads
            var stackFrameInfos = frames.Select(x => GenerateStackTraceFrameInfo(x, config))
                                        .Where(x => !x.Method.StartsWith("Bugsnag.")).ToList();

            return new ExceptionInfo
            {
                ExceptionClass = exception.GetType().Name,
                Description = exception.Message,
                StackTrace = stackFrameInfos
            };
        }

        /// <summary>
        /// Creates a stack frame information payload to use in notifications
        /// </summary>
        /// <param name="frame">The stack frame to base the payload on</param>
        /// <param name="config">The configuration to use</param>
        /// <returns>The stack frame payload to use in notifications</returns>
        public static StackTraceFrameInfo GenerateStackTraceFrameInfo(StackFrame frame, Configuration config)
        {
            if (frame == null || config == null)
                return null;

            // Get the filename the frame comes from without prefixes
            var file = config.RemoveFileNamePrefix(frame.GetFileName());

            // Mark the frame is In Project if we are autodetecting and there is a filename, or 
            // the method comes from the configured project namespaces.
            var method = frame.GetMethod();
            var inProject = (config.AutoDetectInProject && !string.IsNullOrEmpty(file)) ||
                            (method.DeclaringType != null && config.IsInProjectNamespace(method.DeclaringType.FullName));

            return new StackTraceFrameInfo
            {
                File = file,
                LineNumber = frame.GetFileLineNumber(),
                Method = GenerateMethodSignature(method),
                InProject = inProject
            };
        }

        /// <summary>
        /// Generates a method signature from the method
        /// </summary>
        /// <param name="method">The method to create the signature from</param>
        /// <returns>The signature of the method</returns>
        public static string GenerateMethodSignature(MethodBase method)
        {
            if (method == null)
                return null;

            // Create a basic signature using the methods parameters and name
            var param = method.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name);
            var paramSummary = String.Join(", ", param.ToArray());
            var baseSignature = String.Format("{0}({1})", method.Name, paramSummary);

            // Add on the namespace the method came from
            var nameSpace = method.DeclaringType == null ? string.Empty : method.DeclaringType.FullName + ".";
            return nameSpace + baseSignature;
        }
    }
}
