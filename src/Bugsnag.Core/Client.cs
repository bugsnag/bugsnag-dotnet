using System;

namespace Bugsnag.Core
{
    /// <summary>
    /// The main class used to encapsulate a single client to Bugsnag
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The notifier used by the client to send notifications to Bugsnag
        /// </summary>
        private Notifier notifier;

        /// <summary>
        /// The handler used to handle app level exceptions and notify Bugsnag accordingly
        /// </summary>
        private ExceptionHandler exceptionHandler;

        /// <summary>
        /// Gets the configuration of the client, allowing users to config it
        /// </summary>
        public Configuration Config { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class. Will use all the default settings and will 
        /// automatically hook into uncaught exceptions.
        /// </summary>
        /// <param name="apiKey">The Bugsnag API key to send notifications with</param>
        public Client(string apiKey)
            : this(apiKey, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class. Provides the option to automatically 
        /// hook into uncaught exceptions 
        /// </summary>
        /// <param name="apiKey">The Bugsnag API key to use</param>
        /// <param name="autoNotify">True if the client will automatically notify uncaught exceptions, otherwise false</param>
        public Client(string apiKey, bool autoNotify)
        {
            // TODO : Anyway to better validate key other than checking if its null or empty
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("You must provide a Bugsnag API key");

            Config = new Configuration(apiKey);
            notifier = new Notifier(Config);
            exceptionHandler = new ExceptionHandler();

            // Install a default exception handler with this client
            if (autoNotify)
                StartAutoNotify();
        }

        /// <summary>
        /// Enables auto notification, using the default exception handler
        /// </summary>
        public void StartAutoNotify()
        {
            exceptionHandler.InstallHandler(HandleDefaultException);
        }

        /// <summary>
        /// Disables auto notification, removing the handler
        /// </summary>
        public void StopAutoNotify()
        {
            exceptionHandler.UninstallHandler();
        }

        /// <summary>
        /// Notifies Bugsnag of an exception
        /// </summary>
        /// <param name="exception">The exception to send to Bugsnag</param>
        public void Notify(Exception exception)
        {
            var error = new Event(exception);
            Notify(error);
        }

        /// <summary>
        /// Notifies Bugsnag of an exception, with an associated severity level
        /// </summary>
        /// <param name="exception">The exception to send to Bugsnag</param>
        /// <param name="severity">The associated severity of the exception</param>
        public void Notify(Exception exception, Severity severity)
        {
            var error = new Event(exception);
            error.Severity = severity;
            Notify(error);
        }

        /// <summary>
        /// Notifies Bugsnag of an exception with associated meta data
        /// </summary>
        /// <param name="exception">The exception to send to Bugsnag</param>
        /// <param name="data">The metadata to send with the exception</param>
        public void Notify(Exception exception, Metadata data)
        {
            var error = new Event(exception);
            error.Metadata.AddMetadata(data);
            Notify(error);
        }

        /// <summary>
        /// Notifies Bugsnag of an exception, with an associated severity level and meta data
        /// </summary>
        /// <param name="exception">The exception to send to Bugsnag</param>
        /// <param name="severity">The associated severity of the exception</param>
        /// <param name="data">The metadata to send with the exception</param>
        public void Notify(Exception exception, Severity severity, Metadata data)
        {
            var error = new Event(exception);
            error.Severity = severity;
            error.Metadata.AddMetadata(data);
            Notify(error);
        }

        /// <summary>
        /// Notifies Bugsnag of an error event
        /// </summary>
        /// <param name="errorEvent">The event to report on</param>
        public void Notify(Event errorEvent)
        {
            // Do nothing if we don't have an error event
            if (errorEvent == null)
                return;

            // Do nothing if we are not a release stage that notifies
            if (!Config.IsNotifyReleaseStage())
                return;

            // Call the before notify action is there is one
            if (Config.BeforeNotifyFunc != null)
            {
                // Do nothing if the before notify action indicates we should ignore the error event
                var shouldIgnore = Config.BeforeNotifyFunc(errorEvent);
                if (shouldIgnore)
                    return;
            }

            // Ignore the error if the exception it contains is one of the classes to ignore
            if (errorEvent.Exception != null &&
                errorEvent.Exception.GetType() != null &&
                Config.IsClassToIgnore(errorEvent.Exception.GetType().Name))
                return;

            notifier.Send(errorEvent);
        }

        /// <summary>
        /// The default handler to use when we receive unmanaged exceptions
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="runtimeEnding">True if the unmanaged exception handler indicates that the runtime will end</param>
        private void HandleDefaultException(Exception exception, bool runtimeEnding)
        {
            var error = new Event(exception, runtimeEnding);
            Notify(error);
        }
    }
}
