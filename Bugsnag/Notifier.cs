using Bugsnag.Message;
using Bugsnag.Message.App;
using Bugsnag.Message.Core;
using Bugsnag.Message.Device;
using Bugsnag.Message.Event;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;

namespace Bugsnag
{
    public class Notifier
    {
        private Configuration Config { get; set; }
        private static IWebProxy Proxy = WebRequest.DefaultWebProxy;

        public Notifier(Configuration config)
        {
            Config = config;
        }

        public Notifier(string apiKey)
        {
            Config = new Configuration(apiKey);
        }

        public void Send(Exception exp)
        {
            var appInfo = new AppInfo 
            {
                Version = Config.AppVersion,
                ReleaseStage = Config.ReleaseStage
            };

            var userInfo = new UserInfo 
            {
                Id = "ben.ibinson",
                Email = "ben@ibinson.com",
                Name = "Ben Ibinson"
            };

            var events = new List<EventInfo>();

            var expInfos = new List<ExceptionInfo>();
            var currentExp = exp;
            while (currentExp != null)
            {

                var stack = new StackTrace(currentExp, true);
                List<StackTraceFrameInfo> frames = null;
                if (stack.GetFrames() != null)
                {
                    frames = new List<StackTraceFrameInfo>();
                    foreach (var frame in stack.GetFrames())
                    {
                        var method = frame.GetMethod();

                        String[] param = method.GetParameters()
                              .Select(p => String.Format("{0} {1}", p.ParameterType.Name, p.Name))
                              .ToArray();

                        string signature = String.Format("{0}({1})", method.Name, String.Join(",", param));

                        var methodInfo = method.DeclaringType == null ? "" : method.DeclaringType.FullName;
                        methodInfo += "." + signature;

                        var file = frame.GetFileName();
                        if (!String.IsNullOrEmpty(file))
                            file = file.Replace(@"e:\GitHub\Bugsnag-NET\BugsnagDemoMVC\", "");

                        var frameInfo = new StackTraceFrameInfo
                        {
                            File = file,
                            LineNumber = frame.GetFileLineNumber(),
                            Method = methodInfo,
                            InProject = !String.IsNullOrEmpty(file)
                        };
                        frames.Add(frameInfo);
                    }
                }
                var expInfo = new ExceptionInfo
                {
                    ExceptionClass = currentExp.GetType().Name,
                    Message = currentExp.Message,
                    StackTrace = frames
                };
                expInfos.Add(expInfo);
                currentExp = currentExp.InnerException;
            }

            var t1 = new TabInfo
            {
                Entries = new List<TabEntry>
                {
                    new TabKeyValuePair("aaa","Hello_A"),
                    new TabKeyValuePair("bbb","Hello_B"),
                   
                    new TabKeyValuePair("eee","Hello_E"),
                    new TabKeyValuePair("fff","Hello_F"),
                    new TabSection { Name = "Section Alpha", Pairs = new List<TabKeyValuePair>
                    {
                        new TabKeyValuePair("ccc","Hello_C"),
                        new TabKeyValuePair("ddd","Hello_D")
                    }},
                    new TabSection { Name = "Section Beta", Pairs = new List<TabKeyValuePair>
                    {
                        new TabKeyValuePair("ccc","Hello_C"),
                        new TabKeyValuePair("ddd","Hello_D")
                    }}
                }
            };

            var errorInfo = new EventInfo
            {
                App = appInfo,
                Device = Configuration.DeviceInfo,
                Severity = "info",
                User = userInfo,
                Exceptions = expInfos,
                Context = "",
                GroupingHash = "",
                MetaData = new Dictionary<string, TabInfo> { {"Test23", t1 }, {"pop", t2}}
            };
            events.Add(errorInfo);

            var notification = new Notification
            {
                ApiKey = Config.ApiKey,
                Notifier = Configuration.NotifierInfo,
                Events = events
            };

            Send(notification);
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
