using System;
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
      var server = new TestServer(5001, 20);
      server.Start();

      var endpoint = new Uri("http://localhost:5001");
      var transport = new Transport();
      for (int i = 0; i < 20; i++)
      {
        var rawPayload = System.Text.Encoding.UTF8.GetBytes($"{{ \"count\": {i} }}");
        var responseCode = await Task.Factory.FromAsync((callback, state) => transport.BeginSend(endpoint, rawPayload, callback, state), transport.EndSend, null);
        Assert.Equal(HttpStatusCode.OK, responseCode);
      }

      var requests = await server.Requests();

      Assert.Equal(20, requests.Count());
    }
  }
}
