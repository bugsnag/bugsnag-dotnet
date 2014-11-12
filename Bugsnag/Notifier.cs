using Bugsnag.Event;
using Bugsnag.Payload;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Bugsnag
{
    public class Notifier
    {
        public const string Name = ".NET Bugsnag Notifier (ALPHA)";
        public const string Url = "https://bugsnag.com";

        public static readonly string Version;

        private static readonly IWebProxy DetectedProxy;
        private static readonly JsonSerializerSettings JsonSettings;
        
        private Configuration Config { get; set; }
        private NotificationFactory Factory { get; set; }

        static Notifier()
        {
            Version = Assembly.GetExecutingAssembly()
                              .GetName()
                              .Version.ToString(3);

            DetectedProxy = WebRequest.DefaultWebProxy;
            JsonSettings = new JsonSerializerSettings 
            { 
                NullValueHandling = NullValueHandling.Ignore, 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public Notifier(Configuration config)
        {
            Config = config;
            Factory = new NotificationFactory(config);
        }

        public void Send(Error error)
        {
            var notification = Factory.CreateFromError(error);
            if (notification == null)
                Send(notification);            
        }

        private void Send(Notification notification)
        {
            //  Post JSON to server:
            var request = WebRequest.Create(Config.FinalUrl);

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
