using System.Linq;
using Bugsnag.Payload;
using Xunit;

namespace Bugsnag.Tests.Payload
{
  public class EventTests
  {
    [Fact]
    public void HasExpectedPayloadVersion()
    {
      var configuration = new Configuration("123456");
      var exception = new System.DllNotFoundException();
      var severity = Bugsnag.Payload.Severity.ForUnhandledException();
      var breadcrumbs = Enumerable.Empty<Breadcrumb>();

      var @event = new Event(configuration, exception, severity, breadcrumbs);

      Assert.Equal(4, @event["payloadVersion"]);
    }

    [Fact]
    public void SeverityKeysAreAddedCorrectly()
    {
      var configuration = new Configuration("123456");
      var exception = new System.DllNotFoundException();
      var severity = Bugsnag.Payload.Severity.ForUnhandledException();
      var breadcrumbs = Enumerable.Empty<Breadcrumb>();

      var @event = new Event(configuration, exception, severity, breadcrumbs);

      foreach (var key in severity.Keys)
      {
        Assert.Contains(key, @event.Keys);
      }
    }
  }
}
