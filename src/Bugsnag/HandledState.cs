using System;

namespace Bugsnag
{
    public class HandledState
    {
        public Severity OriginalSeverity { get; }
        public Severity CurrentSeverity { get; set; }
        public SeverityReason SeverityReason { get; protected internal set; }
        public bool Unhandled { get; }
        
        public HandledState(SeverityReason reason) 
            : this(reason, reason.DefaultSeverity())
        {
        }

        public HandledState(SeverityReason reason, Severity severity)
        {
            OriginalSeverity = severity;
            CurrentSeverity = severity;
            SeverityReason = reason;
            Unhandled = reason.Unhandled();
        }
    }
}
