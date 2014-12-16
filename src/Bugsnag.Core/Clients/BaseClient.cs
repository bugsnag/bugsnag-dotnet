using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using System.Deployment.Application;
using Bugsnag.ConfigurationStorage;

namespace Bugsnag.Clients
{
    /// <summary>
    /// The main class used to encapsulate a client to Bugsnag
    /// </summary>
    public class BaseClient
    {
        /// <summary>
        /// The notifier used by the client to send notifications to Bugsnag
        /// </summary>
        internal Notifier notifier;

        /// <summary>
        /// The handler used to handle app level exceptions and notify Bugsnag accordingly
        /// </summary>
        internal ExceptionHandler exceptionHandler;

        /// <summary>
        /// Gets the configuration of the client, allowing users to config it
        /// </summary>
        public Configuration Config { get; private set; }

        /// <summary>
        /// The regex that validates an API key
        /// </summary>
        private Regex apiRegex = new Regex("^[a-fA-F0-9]{32}$");

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClient"/> class. Will use all the default settings and will 
        /// automatically hook into uncaught exceptions.
        /// </summary>
        /// <param name="apiKey">The Bugsnag API key to send notifications with</param>
        public BaseClient(string apiKey)
            : this(new BaseStorage(apiKey))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClient"/> class. Provides the option to automatically 
        /// hook into uncaught exceptions. Allows injection of dependant classes
        /// </summary>
        /// <param name="configStorage">The configuration of the client</param>
        public BaseClient(IConfigurationStorage configStorage)
        {
            Initialize(configStorage);
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
            error.Severity = Severity.Warning;
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
            error.Severity = Severity.Warning;
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

            // Ignore the error if the exception it contains is one of the classes to ignore
            if (errorEvent.Exception == null ||
                Config.IsClassToIgnore(errorEvent.Exception.GetType().Name))
                return;

            Config.RunInternalBeforeNotifyCallbacks(errorEvent);
            Config.AddConfigToEvent(errorEvent);

            if (!Config.RunBeforeNotifyCallbacks(errorEvent))
                return;

            notifier.Send(errorEvent);
        }

        /// <summary>
        /// Initialize the client with dependencies
        /// </summary>
        /// <param name="configStorage">The configuration to use</param>
        protected void Initialize(IConfigurationStorage configStorage)
        {
            if (configStorage == null || string.IsNullOrEmpty(configStorage.ApiKey) || !apiRegex.IsMatch(configStorage.ApiKey))
            {
                Logger.Error("You must provide a valid Bugsnag API key");
            }
            else
            {
                Config = new Configuration(configStorage);
                notifier = new Notifier(Config);
                exceptionHandler = new ExceptionHandler();

                // Install a default exception handler with this client
                if (Config.AutoNotify)
                    StartAutoNotify();

                //// Set up some defaults for all clients
                if (Debugger.IsAttached && String.IsNullOrEmpty(Config.ReleaseStage)) Config.ReleaseStage = "development";
                if (String.IsNullOrEmpty(Config.AppVersion))
                {
                    if (ApplicationDeployment.IsNetworkDeployed)
                    {
                        // Use the application version defined for the Click-Once application, if it is one
                        Config.AppVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                    }
                    else if (Assembly.GetEntryAssembly() != null)
                    {
                        Config.AppVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
                    }
                }

                Initialized();
            }
        }

        /// <summary>
        /// Allows subclasses to have a centralized initialize function which is called once the base
        /// client has finished initializing.
        /// </summary>
        protected void Initialized()
        {
            // The base client doesn't need any further initialisation
        }

        /// <summary>
        /// The default handler to use when we receive unmanaged exceptions
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="runtimeEnding">True if the unmanaged exception handler indicates that the runtime will end</param>
        protected void HandleDefaultException(Exception exception, bool runtimeEnding)
        {
            var error = new Event(exception, runtimeEnding);
            Notify(error);
        }
    }
}
