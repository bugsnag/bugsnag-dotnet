using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Bugsnag.Tests
{
  public class TransportTests
  {
    private class TransportTestsState
    {
      public Transport Transport { get; set; }
    }

    [Fact]
    public async void Test()
    {
      var endpoint = new Uri("https://notify.bugsnag.com");
      var transport = new Transport();
      var rawPayload = System.Text.Encoding.UTF8.GetBytes("{}");
      for (int i = 0; i < 20; i++)
      {
        var responseCode = await Task.Factory.FromAsync((callback, state) => transport.BeginSend(endpoint, rawPayload, callback, state), transport.EndSend, null);
        Assert.Equal(HttpStatusCode.OK, responseCode);
      }
    }
  }
}
