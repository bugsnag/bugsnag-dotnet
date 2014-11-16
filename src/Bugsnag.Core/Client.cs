using System;

namespace Bugsnag.Core
{
    public class Client
    {
        public Configuration Config { get; private set; }
        private Notifier Notifier { get; set; }

        public Client(string apiKey, bool installDefaultHandler = true)
        {
            if (String.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException("You must provide a Bugsnag API key");

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

        public void StartAutoNotify(Action<Exception, bool> customHandler = null)
        {
            if (customHandler == null)
                ExceptionHandler.InstallDefaultHandler(HandleDefaultException);
            else
                ExceptionHandler.InstallDefaultHandler(customHandler);
        }

        public void StopAutoNotify()
        {
            ExceptionHandler.UninstallDefaultHandler();
        }


        public void Notify(Exception exp)
        {
            var error = new Event(exp);
            Notify(error);
        }

        public void Notify(Exception exp, Severity severity)
        {
            var error = new Event(exp);
            error.Severity = severity;
            Notify(error);
        }

        public void Notify(Event err)
        {
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
