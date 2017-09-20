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
        internal StackTrace CallTrace { get; private set; }

        /// <summary>
        /// Gets or sets the grouping hash value used to identify similar events
        /// </summary>
        public string GroupingHash { get; set; }

        /// <summary>
        /// Gets or sets the severity of the event
        /// </summary>
        public Severity Severity
        {
            get { return HandledState.CurrentSeverity; }
            set { HandledState.CurrentSeverity = value; }
        }

        /// <summary>
        /// Gets or set the rationale for error severity and unhandled status
        /// </summary>
        internal HandledState HandledState { get; }

        /// <summary>
        /// Gets or sets the additional data to be sent with the event
        /// </summary>
        public Metadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the Context associated with this error.
        /// </summary>
        public String Context { get; set; }

        /// <summary>
        /// Gets or sets the user id associated with this error.
        /// </summary>
        public String UserId { get; set; }

        /// <summary>
        /// Gets or sets the user name associated with this error.
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// Gets or sets the user email associated with this error.
        /// </summary>
        public String UserEmail { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class. Assumes runtime is not ending.
        /// </summary>
        /// <param name="exception">The exception to report on</param>
        public Event(Exception exception)
            : this(exception, false, new HandledState(SeverityReason.HandledException), 2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="exception">The exception to report on</param>
        /// <param name="runtimeEnding">True if the runtime is ending otherwise false</param>
        public Event(Exception exception, bool runtimeEnding)
            : this(exception, runtimeEnding, new HandledState(SeverityReason.HandledException), 2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="exception">The exception to report on</param>
        /// <param name="runtimeEnding">True if the runtime is ending otherwise false</param>
        public Event(Exception exception, bool runtimeEnding, HandledState handledState)
            : this(exception, runtimeEnding, handledState, 2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="exception">The exception to report on</param>
        /// <param name="runtimeEnding">True if the runtime is ending otherwise false</param>
        protected Event(Exception exception, bool runtimeEnding, HandledState handledState, int skippedFrames)
        {
            HandledState = handledState;
            // Record a full notify stack trace if the exception has none (ignoring the first constructor stack frame)
            if (exception == null || exception.StackTrace == null)
                CallTrace = new StackTrace(skippedFrames, true);

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
            Metadata = new Metadata();
        }
    }
}
