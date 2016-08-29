using System;
using System.IO;
using System.Net;
using System.Reflection;
using Bugsnag.Payload;
using Newtonsoft.Json;
using System.IO.IsolatedStorage;

namespace Bugsnag
{
    internal class Notifier
    {
        public const string Name = ".NET Bugsnag Notifier";
        public static readonly Uri Url = new Uri("https://github.com/bugsnag/bugsnag-net");

        public static readonly string Version =
            Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        private static readonly IWebProxy DetectedProxy = WebRequest.DefaultWebProxy;
        private static readonly JsonSerializerSettings JsonSettings =
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        private Configuration Config { get; set; }
        private NotificationFactory Factory { get; set; }
        private OfflineStore Store { get; set; }

        public Notifier(Configuration config)
        {
            Config = config;
            Factory = new NotificationFactory(config);
            Store = new OfflineStore();
        }

        public void Send(Event errorEvent)
        {
            var notification = Factory.CreateFromError(errorEvent);
            if (notification != null)
                Send(notification);
        }

        public void SendStoredReports()
        {
            try
            {
                foreach (var storedReport in Store.ReadStoredJson())
                {
                    SendJson(storedReport);
                }
            }
            catch (Exception e)
            {
                Logger.Warning(string.Format("Bugsnag failed to send persisted error reports with exception: {0}", e.ToString()));
            }
        }

        private void Send(Notification notification)
        {
            try
            {
                SendJson(JsonConvert.SerializeObject(notification, JsonSettings));
            }
            catch (Exception e)
            {
                Logger.Warning(string.Format("Bugsnag failed to serialise error report with exception: {0}", e.ToString()));
            }
        }

        private void SendJson(string json)
        {
            var reportSent = false;
            try
            {
                //  Post JSON to server:
                var request = WebRequest.Create(Config.EndpointUrl);

                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/json";
                request.Proxy = DetectedProxy;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                using (request.GetResponse())
                {
                }
                reportSent = true;
            }
            catch (Exception e)
            {
                Logger.Warning("Bugsnag failed to send error report with exception: " + e.ToString());
            }
            if (!reportSent)
            {
                try
                {
                    Store.StoreJson(json);
                }
                catch (Exception e)
                {
                    Logger.Warning("Bugsnag failed to persist error report with exception: " + e.ToString());
                }
            }
        }
    }
}
