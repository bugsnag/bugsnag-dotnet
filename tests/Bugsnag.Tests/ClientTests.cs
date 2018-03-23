using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Bugsnag.Tests
{
  public class ClientTests
  {
    [Fact]
    public async void TestThrownException()
    {
      var server = new TestServer(1);

      server.Start();

      var metadata = new Dictionary<string, object>() { { "password", "secret" } };
      var subMetadata = new Dictionary<string, object>() { { "circular", metadata }, { "password", "not again!" }, { "uri", new Uri("http://google.com?password=1111&wow=cool") } };
      metadata["test"] = subMetadata;
      var filters = new string[] { "password" };

      var client = new Client(new Configuration("123456") { Endpoint = server.Endpoint, MetadataFilters = filters });

      client.BeforeNotify(r => {
        r.Event.Metadata["bugsnag"] = metadata;
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

      Assert.Single(requests);
    }

    public class NonThrownException : IAsyncLifetime
    {
      public string BugsnagPayload { get; set; }

      public Task DisposeAsync()
      {
        return Task.CompletedTask;
      }

      public async Task InitializeAsync()
      {
        var server = new TestServer(1);

        server.Start();

        var metadata = new Dictionary<string, object>() { { "password", "secret" } };
        var subMetadata = new Dictionary<string, object>() { { "circular", metadata }, { "password", "not again!" }, { "uri", new Uri("http://google.com?password=1111&wow=cool") } };
        metadata["test"] = subMetadata;
        var filters = new string[] { "password" };

        var client = new Client(new Configuration("123456") { Endpoint = server.Endpoint, MetadataFilters = filters });

        client.BeforeNotify(r => {
          r.Event.Metadata["bugsnag"] = metadata;
        });

        TestNotifyMethod(client);

        var requests = await server.Requests();

        BugsnagPayload = requests.Single();
      }

      /// <summary>
      /// A method that we can use to check that the stack trace contains the
      /// initial notify call method.
      /// </summary>
      /// <param name="client"></param>
      [MethodImpl(MethodImplOptions.NoInlining)]
      private void TestNotifyMethod(IClient client)
      {
        client.Notify(new ArgumentNullException());
      }

      /// <summary>
      /// The stack trace should contain the notify method above.
      /// </summary>
      [Fact]
      public void ContainsExternalNotifyMethod()
      {
        Assert.Contains("Bugsnag.Tests.ClientTests+NonThrownException.TestNotifyMethod(IClient client)", BugsnagPayload);
      }

      /// <summary>
      /// The stack trace should not contain the internal notify call.
      /// </summary>
      [Fact]
      public void DoesNotContainInternalNotifyMethod()
      {
        Assert.DoesNotContain("Bugsnag.Client.Notify", BugsnagPayload);
      }
    }
  }
}
