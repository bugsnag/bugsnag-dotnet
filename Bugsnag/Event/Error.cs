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
            
            // Get to the inner most exception
            var innerExp = exp;
            while (innerExp.InnerException != null)
                innerExp = innerExp.InnerException;

            // Record the exception details if there are any
            if (runtimeEnding != null)
                MetaData.AddToTab("Exception Details", "runtimeEnding", IsRuntimeEnding);

            if (innerExp.HelpLink != null)
                MetaData.AddToTab("Exception Details", "helpLink", innerExp.HelpLink);

            if (innerExp.Source != null)
                MetaData.AddToTab("Exception Details", "source", innerExp.Source);

            if (innerExp.TargetSite != null)
                MetaData.AddToTab("Exception Details", "targetSite", innerExp.TargetSite);

            // Record a full notify stack trace if the exception has none
            if (Exception.StackTrace == null)
                CallTrace = new StackTrace(1, true);
        }
    }
}
