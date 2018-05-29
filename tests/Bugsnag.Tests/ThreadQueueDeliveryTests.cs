using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace Bugsnag.Tests
{
  public class ThreadQueueDeliveryTests
  {
    [Fact]
    public async void Test()
    {
      var numberOfRequests = 500;

      var server = new TestServer();

      server.Start();

      for (int i = 0; i < numberOfRequests; i++)
      {
        var payload = new SamplePayload(i, server.Endpoint);
        ThreadQueueDelivery.Instance.Send(payload);
      }

      var requests = await server.Requests(numberOfRequests);

      Assert.Equal(numberOfRequests, requests.Count());
    }

    private class SamplePayload : IPayload
    {
      public SamplePayload(int count, Uri endpoint)
      {
        Count = count;
        Endpoint = endpoint;
      }

      public Uri Endpoint { get; set; }

      public IWebProxy Proxy { get; set; }

      public KeyValuePair<string, string>[] Headers => new KeyValuePair<string, string>[] { };

      public int Count { get; set; }

      public byte[] Serialize()
      {
        return System.Text.Encoding.UTF8.GetBytes($"payload {Count}");
      }
    }
  }
}
