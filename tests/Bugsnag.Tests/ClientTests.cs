using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bugsnag.Tests
{
  public class ClientTests
  {
    [Fact]
    public async void Test()
    {
      var server = new TestServer(5002, 1);

      server.Start();

      var endpoint = new Uri("http://localhost:5002");
      var metadata = new Dictionary<string, object>() { { "password", "secret" } };
      var subMetadata = new Dictionary<string, object>() { { "circular", metadata }, { "password", "not again!" }, { "uri", new Uri("http://google.com?password=1111&wow=cool") } };
      metadata["test"] = subMetadata;
      var filters = new string[] { "password" };

      var client = new Client(new Configuration("123456") { Endpoint = endpoint, MetadataFilters = filters });

      client.BeforeNotify((c, r) => {
        foreach (var @event in r.Events)
        {
          @event.Metadata["bugsnag"] = metadata;
        }
      });

      try
      {
        throw new ArgumentNullException();
      }
      catch (Exception e)
      {
        client.Notify(e);
      }

      var requests = await server.Requests();

      Assert.Equal(1, requests.Count());
    }
  }
}
