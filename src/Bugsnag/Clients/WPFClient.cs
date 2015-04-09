using System;
using System.Linq;
using System.Windows;

#if !NET35
using System.Threading.Tasks;
#endif

namespace Bugsnag.Clients
{
    public static class WPFClient
    {
        public static Configuration Config;
        private static BaseClient Client;

        static WPFClient()
        {
            Client = new BaseClient(ConfigurationStorage.ConfigSection.Settings);
            Config = Client.Config;
            Client.Config.BeforeNotify(error =>
            {
                var currWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                if (currWindow != null)
                {
                    error.Context = currWindow.Title;
                }
            });
        }

        public static void Start()
        {

        }

        public static void Notify(Exception error)
        {
            Client.Notify(error);
        }

        public static void Notify(Exception error, Metadata metadata)
        {
            Client.Notify(error, metadata);
        }

        public static void Notify(Exception error, Severity severity)
        {
            Client.Notify(error, severity);
        }

        public static void Notify(Exception error, Severity severity, Metadata metadata)
        {
            Client.Notify(error, severity, metadata);
        }

#if !NET35
        public static Task NotifyAsync(Exception error, Metadata metadata)
        {
            return Task.Factory.StartNew(() =>
                {
                    Client.Notify(error, metadata);
                });
        }

        public static Task NotifyAsync(Exception error, Severity severity)
        {
            return Task.Factory.StartNew(() =>
                {
                    Client.Notify(error, severity);
                });
        }

        public static Task NotifyAsync(Exception error, Severity severity, Metadata metadata)
        {
            return Task.Factory.StartNew(() =>
                {
                    Client.Notify(error, severity, metadata);
                });
        }
#endif
    }
}
