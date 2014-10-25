using Bugsnag.Event;
using Bugsnag.Message;
using Bugsnag.Message.Core;
using Bugsnag.Message.Device;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace Bugsnag
{
    public class Notifier
    {
        private Configuration Config { get; set; }

        private static readonly IWebProxy Proxy;
        public static readonly NotifierInfo NotifierInfo;
        public static readonly DeviceInfo DeviceInfo;

        private const string DefaultEndpoint = "notify.bugsnag.com";

        static Notifier()
        {
            NotifierInfo = new NotifierInfo
            {
                Name = ".NET Bugsnag Notifier (ALPHA)",
                Version = "1.2.6",
                Url = "https://bugsnag.com"
            };

            DeviceInfo = new DeviceInfo
            {
                OsVersion = Profiler.GetOsInfo()
            };

            Proxy =  WebRequest.DefaultWebProxy;
        }

        public Notifier(Configuration config)
        {
            Config = config;
        }

        public void Send(Exception exp, bool? runtimeEnding = null)
        {
            var error = new Error(exp, runtimeEnding);
            Send(error);
        }

        public void Send(Error error)
        {
            error.MetaData.AddToTab("Exp", "Help Links", new { Exception1 = "http://google.com", Exception2 = "No Idea" });
            error.MetaData.AddToTab("Runtime Dying", true);
            var notification = NotificationFactory.CreateFromError(error, Config);
            Send(notification);


            //var addErrorTab = new Dictionary<string, object>
            //{ 
            //    { "lmo", "lmo2" },
            //    { "raman", true},
            //    { "dfd", new
            //             {
            //                looper = "loops",
            //                jump = new
            //                {
            //                    a = 234,
            //                    b = 245,
            //                    c = 234
            //                }
            //            }}
            //};


            
        }


        public void Send(Notification notification)
        {
            //  Post JSON to server:
            //var request = WebRequest.Create("http://requestb.in/12pa7di1");
            var request = WebRequest.Create("https://notify.bugsnag.com");

            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";
            request.Proxy = Proxy;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                streamWriter.Write(json);
                streamWriter.Flush();
            }
            request.GetResponse().Close();
        } 
    }
}
