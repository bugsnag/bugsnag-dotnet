using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using Bugsnag.Clients;
using Newtonsoft.Json.Linq;

namespace Bugsnag.Test.FunctionalTests
{
    public class TestServer : IDisposable
    {
        private HttpListener listener;
        private ConcurrentQueue<JObject> messageQueue;

        public TestServer(BaseClient client)
        {
            client.Config.Endpoint = "http://localhost:8181/";
            messageQueue = new ConcurrentQueue<JObject>();
            Start();
        }

        public async void Start()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8181/");
            listener.Start();

            while (true)
            {
                var context = await listener.GetContextAsync();
                try
                {
                    messageQueue.Enqueue(ProcessJsonRequest(context));
                }
                finally
                {
                    context.Response.StatusCode = 200;
                    context.Response.Close();
                }
            }
        }

        public void Stop()
        {
            if (listener != null)
                listener.Abort();
        }

        private JObject ProcessJsonRequest(HttpListenerContext context)
        {
            string body = "";
            using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                body = reader.ReadToEnd();
            }
            return JObject.Parse(body);
        }

        public JObject GetLastResponse()
        {
            JObject result = null;
            messageQueue.TryDequeue(out result);
            return result;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
