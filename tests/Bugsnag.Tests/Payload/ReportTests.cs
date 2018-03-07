using Bugsnag.Payload;
using Xunit;

namespace Bugsnag.Tests.Payload
{
  public class ReportTests
  {
    [Fact]
    public void ContainsTheRequiredKeys()
    {
      var configuration = new Configuration("123456");
      var exception = new System.DllNotFoundException();
      var severity = Bugsnag.Payload.HandledState.ForUnhandledException();
      var breadcrumbs = new Breadcrumb[0];
      var session = new Session();
      var request = new Request();

      var @event = new Report(configuration, exception, severity, breadcrumbs, session, request);

      Assert.Equal(configuration.ApiKey, @event["apiKey"]);
      Assert.NotNull(@event["notifier"]);
      Assert.NotNull(@event["events"]);
    }
  }
}
