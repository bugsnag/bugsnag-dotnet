using Bugsnag.Event;
using Bugsnag.Message;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Web;
using System.Linq;
using System.Collections.Generic;

namespace Bugsnag
{
    public class Notifier
    {
        public const string Name = ".NET Bugsnag Notifier (ALPHA)";
        public const string Version = "1.2.6";
        public const string Url = "https://bugsnag.com";

        private const string DefaultEndpoint = "http://notify.bugsnag.com";
        private const string DefaultEndpointSsl = "https://notify.bugsnag.com";

        private static readonly IWebProxy DetectedProxy;
        private static readonly JsonSerializerSettings JsonSettings;

        private Configuration Config { get; set; }

        static Notifier()
        {
            DetectedProxy = WebRequest.DefaultWebProxy;
            JsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }

        public Notifier(Configuration config)
        {
            Config = config;
        }

        public void Send(Error error)
        {
            if (Config.BeforeNotifyFunc != null && !Config.BeforeNotifyFunc(error))
                return;

            var notification = NotificationFactory.CreateFromError(error, Config);
            Send(notification);            
        }

        private void Send(Notification notification)
        {
            //  Post JSON to server:
            var request = WebRequest.Create(Config.UseSsl ? DefaultEndpointSsl : DefaultEndpoint);

            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";
            request.Proxy = DetectedProxy;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(notification, JsonSettings);
                streamWriter.Write(json);
                streamWriter.Flush();
            }
            request.GetResponse().Close();
        } 
    }
}
