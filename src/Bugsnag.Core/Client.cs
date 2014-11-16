using System;

namespace Bugsnag.Core
{
    public class Client
    {
        public Configuration Config { get; private set; }
        private Notifier Notifier { get; set; }

        public Client(string apiKey) : this(apiKey, true) { }

        public Client(string apiKey, bool installDefaultHandler)
        {
            if (String.IsNullOrEmpty(apiKey))
                throw new ArgumentException("You must provide a Bugsnag API key");

            Config = new Configuration(apiKey);
            Notifier = new Notifier(Config);

            // Install a default exception handler with this client
            if (installDefaultHandler)
                StartAutoNotify();
        }

        public Client(Configuration config)
        {
            Config = config;
            Notifier = new Notifier(Config);
        }

        public void StartAutoNotify()
        {
            ExceptionHandler.InstallDefaultHandler(HandleDefaultException);
        }

        public void StartAutoNotify(Action<Exception, bool> customHandler)
        {
            if (customHandler != null)
                ExceptionHandler.InstallDefaultHandler(customHandler);
        }

        // TODO, this will clear it globally rather than on a client by client basis
        public void StopAutoNotify()
        {
            ExceptionHandler.UninstallDefaultHandler();
        }


        public void Notify(Exception exception)
        {
            var error = new Event(exception);
            Notify(error);
        }

        public void Notify(Exception exception, Severity severity)
        {
            var error = new Event(exception);
            error.Severity = severity;
            Notify(error);
        }

        public void Notify(Event err)
        {
            if (err == null)
                return;

            // Call the before notify action is there is one
            if (Config.BeforeNotifyFunc != null && !Config.BeforeNotifyFunc(err))
                return;

            // Ignore the error if its part of the classes we are ignoring
            if (Config.IgnoreClasses != null &&
                err.Exception != null &&
                err.Exception.GetType() != null &&
                Config.IgnoreClasses.Contains(err.Exception.GetType().Name))
                return;

            Notifier.Send(err);
        }

        private void HandleDefaultException(Exception exp, bool runtimeEnding)
        {
            var error = new Event(exp, runtimeEnding);
            Notify(error);
        }
    }
}
