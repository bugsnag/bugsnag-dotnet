using System;

namespace Bugsnag.Clients
{
    public static class WPFClient
    {
        private static BaseClient Client;
        public static Configuration Config;

        static WPFClient()
        {
            Client = new BaseClient(ConfigurationStorage.XMLStorage.Settings);
            Config = Client.Config;
            Client.Config.BeforeNotify(error =>
            {
                return true;
            });
        }

        public static void Start()
        {

        }

        public static void Notify(Exception error)
        {
            if (Client.Config.AutoNotify)
                Client.Notify(error);
        }
    }
}
