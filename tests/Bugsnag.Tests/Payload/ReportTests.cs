using System.Linq;
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
      var severity = Bugsnag.Payload.Severity.ForUnhandledException();
      var breadcrumbs = Enumerable.Empty<Breadcrumb>();

      var @event = new Report(configuration, exception, severity, breadcrumbs);

      Assert.Equal(configuration.ApiKey, @event["apiKey"]);
      Assert.NotNull(@event["notifier"]);
      Assert.NotNull(@event["events"]);
    }
  }
}
