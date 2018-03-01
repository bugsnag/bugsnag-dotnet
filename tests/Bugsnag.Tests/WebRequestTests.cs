using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Bugsnag.Tests
{
  public class WebRequestTests
  {
    [Fact]
    public async void Test()
    {
      var numerOfRequests = 1;
      var server = new TestServer(numerOfRequests);
      server.Start();

      var webRequest = new WebRequest();

      var headers = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Test-Header", "wow!") };

      var rawPayload = System.Text.Encoding.UTF8.GetBytes($"{{ \"count\": {numerOfRequests} }}");
      var response = await Task.Factory.FromAsync((callback, state) => webRequest.BeginSend(server.Endpoint, null, headers, rawPayload, callback, state), webRequest.EndSend, null);
      Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);

      var requests = await server.Requests();

      Assert.Equal(numerOfRequests, requests.Count());
    }
  }
}
