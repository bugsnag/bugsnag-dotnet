using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Bugsnag.Clients;
using Newtonsoft.Json.Linq;

namespace Bugsnag.Core.Test.FunctionalTests
{
    public class TestServer : IDisposable
    {
        private HttpListener listener;
        public Task<JObject> LastJsonResponse;

        public TestServer(BaseClient client)
        {
            client.Config.Endpoint = "http://localhost:8181/";
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8181/");
            listener.Start();
        }

        public void ListenForResponse()
        {
            LastJsonResponse = listener.GetContextAsync().ContinueWith(x =>
            {
                var json = GetJsonObjectFromRequest(x.Result.Request);
                x.Result.Response.Close();
                return json;
            });
        }

        private JObject GetJsonObjectFromRequest(HttpListenerRequest request)
        {
            string body = "";
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                body = reader.ReadToEnd();
            }
            return JObject.Parse(body);
        }

        public JObject Response()
        {
            LastJsonResponse.Wait(100);
            return LastJsonResponse.Result;
        }

        public void Dispose()
        {
            listener.Abort();
        }
    }
}
