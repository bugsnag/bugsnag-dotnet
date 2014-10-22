using System;

namespace Bugsnag
{
    public class Client
    {
        private Notifier Notifer { get; set; }

        public Client(string apiKey, bool installHandler = true)
        {
            if (String.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException("You must provide a Bugsnag API key");

            Notifer = new Notifier(apiKey);

            // Install a default exception handler with this client
            if (installHandler)
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(NotifyExceptionHandler);
        }

        private void NotifyExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // TODO : Record and feedback that the runtime is exiting
            var exp = e.ExceptionObject as Exception;
            if (exp != null)
                Notify(exp);
        }

        public void Notify(Exception exp)
        {
            Notifer.Send(exp);
        }
    }
}
