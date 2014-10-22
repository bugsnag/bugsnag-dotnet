using Bugsnag.Message;
using Bugsnag.Message.App;
using Bugsnag.Message.Core;
using Bugsnag.Message.Device;
using Bugsnag.Message.Event;
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
        private DataContractJsonSerializer serialiser = new DataContractJsonSerializer(typeof(Notification));

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
                
                var stack = new StackTrace(exp, true);
                var frames = new List<StackTraceFrameInfo>();
                if (stack.GetFrames() != null)
                {
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
                    ExceptionClass = exp.GetType().Name,
                    Message = exp.Message,
                    StackTrace = frames
                };
                expInfos.Add(expInfo);
                currentExp = exp.InnerException;
            }       

            var errorInfo = new EventInfo
            {
                App = appInfo,
                Device = new DeviceInfo { OsVersion = Config.OsVersion },
                Severity = "info",
                User = userInfo,
                Exceptions = expInfos,
                Context = "",
                GroupingHash = ""
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
            var stream = new MemoryStream();
            serialiser.WriteObject(stream, notification);

            //  Create a byte array:
            byte[] byteArray = stream.ToArray();

            //  Post JSON to server:
            //var request = WebRequest.Create("http://requestb.in/12pa7di1");
            var request = WebRequest.Create("https://notify.bugsnag.com");
           
            //if (useSSL)
            //    request = WebRequest.Create(httpsUrl);
            //else
            //    request = WebRequest.Create(httpUrl);

            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //  Get the response.  See https://bugsnag.com/docs/notifier-api for response codes
            var response = request.GetResponse();
        }

        
    }
}
