using System;
using System.Diagnostics;

namespace Bugsnag
{
    /// <summary>
    /// Contains all the information needed to report a single exception 
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Gets the exception that this event is about
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the exception has caused the runtime to end
        /// </summary>
        public bool IsRuntimeEnding { get; private set; }

        /// <summary>
        /// Gets the call stack trace of when event was created
        /// </summary>
        public StackTrace CallTrace { get; private set; }

        /// <summary>
        /// Gets or sets the grouping hash value used to identify similar events
        /// </summary>
        public string GroupingHash { get; set; }

        /// <summary>
        /// Gets or sets the severity of the event
        /// </summary>
        public Severity Severity { get; set; }

        /// <summary>
        /// Gets or sets the additional data to be sent with the event
        /// </summary>
        public Metadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the Context associated with this error.
        /// </summary>
        public String Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class. Assumes runtime is not ending.
        /// </summary>
        /// <param name="exception">The exception to report on</param>
        public Event(Exception exception)
        {
            // Record a full notify stack trace if the exception has none (ignoring the first constructor stack frame)
            if (exception == null || exception.StackTrace == null)
                CallTrace = new StackTrace(1, true);

            Intialise(exception, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="exception">The exception to report on</param>
        /// <param name="runtimeEnding">True if the runtime is ending otherwise false</param>
        public Event(Exception exception, bool runtimeEnding)
        {
            // Record a full notify stack trace if the exception has none (ignoring the first constructor stack frame)
            if (exception == null || exception.StackTrace == null)
                CallTrace = new StackTrace(1, true);

            Intialise(exception, runtimeEnding);
        }

        /// <summary>
        /// Initializes a new event instance, ensures the right number of initial call stack frames are ignored
        /// </summary>
        /// <param name="exception">The exception to report on</param>
        /// <param name="runtimeEnding">True if the runtime is ending otherwise false</param>
        protected void Intialise(Exception exception, bool runtimeEnding)
        {
            Exception = exception;
            IsRuntimeEnding = runtimeEnding;
            Severity = Severity.Error;
            Metadata = new Metadata();
        }
    }
}
