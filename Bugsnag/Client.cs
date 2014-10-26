using System;
using System.Diagnostics;

namespace Bugsnag
{
    public class Client
    {
        public Configuration Config { get; private set; }
        private Notifier Notifier { get; set; }

        public Client(string apiKey, bool installHandler = true)
        {
            if (String.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException("You must provide a Bugsnag API key");

            Config = new Configuration(apiKey);
            Notifier = new Notifier(Config);

            // Install a default exception handler with this client
            if (installHandler)
                ExceptionHandler.InstallDefaultHandler(Notifier.Send);
        }

        public void Notify(Exception exp)
        {
            Notifier.Send(exp);
        }
    }
}
