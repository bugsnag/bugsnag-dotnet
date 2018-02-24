using System;
using Xunit;

namespace Bugsnag.Tests
{
  public class SessionTrackingTests
  {
    [Fact]
    public void CurrentSessionIsNullWithoutBeingSet()
    {
      var sessionTracking = new SessionTracker(new Configuration("123456"));

      Assert.Null(sessionTracking.CurrentSession);
    }

    [Fact]
    public async void CurrentSessionCanBeSet()
    {
      var server = new TestServer(1);

      server.Start();

      var sessionTracking = new SessionTracker(new Configuration("123456") { SessionTrackingInterval = TimeSpan.FromSeconds(5), SessionEndpoint = server.Endpoint });

      sessionTracking.CreateSession();

      var requests = await server.Requests();

      Assert.NotNull(sessionTracking.CurrentSession);
    }
  }
}
