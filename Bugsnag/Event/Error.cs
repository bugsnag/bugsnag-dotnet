using System;
using System.Diagnostics;

namespace Bugsnag.Event
{    
    public class Error : Event
    {
        public Exception Exception { get; private set; }
        public bool? IsRuntimeEnding { get; private set; }
        public StackTrace CallTrace { get; set; }

        public Error(Exception exp, bool? runtimeEnding = null): base()
        {
            Exception = exp;
            IsRuntimeEnding = runtimeEnding;
            Severity = Severity.Error;

            // Record a full notify stack trace if the exception has none (ignoring the first constructor stack frame)
            if (Exception.StackTrace == null)
                CallTrace = new StackTrace(1, true);
        }
    }
}
