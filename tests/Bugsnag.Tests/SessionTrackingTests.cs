using System;
using System.Threading;
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
      var server = new TestServer();

      server.Start();

      var sessionTracking = new SessionTracker(new Configuration("123456") {
        SessionEndpoint = server.Endpoint
      });

      sessionTracking.CreateSession();

      var requests = await server.Requests(1);

      Assert.NotNull(sessionTracking.CurrentSession);
    }

    [Fact]
    public async void EmptySessionsPayloadsAreNotSent()
    {
      var server = new TestServer();

      server.Start();

      var sessionTracking = new SessionTracker(new Configuration("123456") {
        SessionEndpoint = server.Endpoint
      });

      sessionTracking.CreateSession();

      // set the cancellation token length long enough so that the session
      // tracker would attempt to send sessions twice
      var cts = new CancellationTokenSource(TimeSpan.FromSeconds(130));

      // the cancellation token should throw as we are 'expecting' 2 requests
      // to be received but we only want one to be sent as the second sessions
      // request will have no sessions data
      await Assert.ThrowsAsync<OperationCanceledException>(async () => {
        await server.Requests(2, cts.Token);
      });
    }
  }
}
