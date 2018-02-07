using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Bugsnag.Tests
{
  public class TransportTests
  {
    [Fact]
    public async void Test()
    {
      var numerOfRequests = 1;
      var server = new TestServer(numerOfRequests);
      server.Start();

      var transport = new Transport();

      var headers = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Test-Header", "wow!") };

      var rawPayload = System.Text.Encoding.UTF8.GetBytes($"{{ \"count\": {numerOfRequests} }}");
      var responseCode = await Task.Factory.FromAsync((callback, state) => transport.BeginSend(server.Endpoint, headers, rawPayload, callback, state), transport.EndSend, null);
      Assert.Equal(HttpStatusCode.OK, responseCode);

      var requests = await server.Requests();

      Assert.Equal(numerOfRequests, requests.Count());
    }
  }
}
