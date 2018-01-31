using System;
using System.Linq;
using Xunit;

namespace Bugsnag.Tests
{
  public class ThreadQueueTransportTests
  {
    [Fact]
    public async void Test()
    {
      var numberOfRequests = 500;

      var server = new TestServer(5000, numberOfRequests);

      server.Start();

      var endpoint = new Uri("http://localhost:5000");

      for (int i = 0; i < numberOfRequests; i++)
      {
        var rawPayload = System.Text.Encoding.UTF8.GetBytes($"{{ \"count\": {i} }}");
        ThreadQueueTransport.Instance.Send(endpoint, rawPayload);
      }

      var requests = await server.Requests();

      Assert.Equal(numberOfRequests, requests.Count());
    }
  }
}
