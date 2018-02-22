using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace Bugsnag.Tests
{
  public class ThreadQueueTransportTests
  {
    [Fact]
    public async void Test()
    {
      var numberOfRequests = 500;

      var server = new TestServer(numberOfRequests);

      server.Start();

      for (int i = 0; i < numberOfRequests; i++)
      {
        var payload = new SamplePayload(i, server.Endpoint);
        ThreadQueueTransport.Instance.Send(payload);
      }

      var requests = await server.Requests();

      Assert.Equal(numberOfRequests, requests.Count());
    }

    private class SamplePayload : Dictionary<string, int>, IPayload
    {
      public SamplePayload(int count, Uri endpoint)
      {
        this["count"] = count;
        Endpoint = endpoint;
      }

      public Uri Endpoint { get; set; }

      public IWebProxy Proxy { get; set; }

      public KeyValuePair<string, string>[] Headers => new KeyValuePair<string, string>[] { };
    }
  }
}
