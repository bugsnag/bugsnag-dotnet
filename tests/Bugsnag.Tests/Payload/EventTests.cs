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
      var app = new App("version", "releaseStage", "type");
      var device = new Device("hostname");
      var exception = new System.DllNotFoundException();
      var severity = Bugsnag.Payload.Severity.ForUnhandledException();
      var breadcrumbs = Enumerable.Empty<Breadcrumb>();
      var session = new Session();
      var request = new Request();

      var payloadVersion = "4";

      var @event = new Event(payloadVersion, app, device, exception, severity, breadcrumbs, session, request);

      Assert.Equal(payloadVersion, @event["payloadVersion"]);
    }

    [Fact]
    public void SeverityKeysAreAddedCorrectly()
    {
      var app = new App("version", "releaseStage", "type");
      var device = new Device("hostname");
      var exception = new System.DllNotFoundException();
      var severity = Bugsnag.Payload.Severity.ForUnhandledException();
      var breadcrumbs = Enumerable.Empty<Breadcrumb>();
      var session = new Session();
      var request = new Request();

      var @event = new Event("1", app, device, exception, severity, breadcrumbs, session, request);

      foreach (var key in severity.Keys)
      {
        Assert.Contains(key, @event.Keys);
      }
    }
  }
}
