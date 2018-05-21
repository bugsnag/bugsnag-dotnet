using System;
using System.Collections.Generic;
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
      var severity = Bugsnag.Payload.HandledState.ForUnhandledException();
      var breadcrumbs = Enumerable.Empty<Breadcrumb>();
      var session = new Session();

      var payloadVersion = "4";

      var @event = new Event(payloadVersion, app, device, exception, severity, breadcrumbs, session);

      Assert.Equal(payloadVersion, @event["payloadVersion"]);
    }

    [Fact]
    public void SeverityKeysAreAddedCorrectly()
    {
      var app = new App("version", "releaseStage", "type");
      var device = new Device("hostname");
      var exception = new System.DllNotFoundException();
      var severity = Bugsnag.Payload.HandledState.ForUnhandledException();
      var breadcrumbs = Enumerable.Empty<Breadcrumb>();
      var session = new Session();

      var @event = new Event("1", app, device, exception, severity, breadcrumbs, session);

      foreach (var key in severity.Keys)
      {
        Assert.Contains(key, @event.Keys);
      }
    }

    [Theory]
    [InlineData(Severity.Error)]
    [InlineData(Severity.Warning)]
    [InlineData(Severity.Info)]
    public void SeverityCanBeRetrieved(Severity severity)
    {
      var app = new App("version", "releaseStage", "type");
      var device = new Device("hostname");
      var exception = new System.DllNotFoundException();
      var breadcrumbs = Enumerable.Empty<Breadcrumb>();
      var session = new Session();
      var handledState = HandledState.ForUserSpecifiedSeverity(severity);

      var @event = new Event("1", app, device, exception, handledState, breadcrumbs, session);

      Assert.Equal(severity, @event.Severity);
    }

    [Theory]
    [InlineData(Severity.Error, Severity.Info)]
    [InlineData(Severity.Warning, Severity.Error)]
    [InlineData(Severity.Info, Severity.Warning)]
    public void SeverityCanBeUpdated(Severity originalSeverity, Severity updatedSeverity)
    {
      var app = new App("version", "releaseStage", "type");
      var device = new Device("hostname");
      var exception = new System.DllNotFoundException();
      var breadcrumbs = Enumerable.Empty<Breadcrumb>();
      var session = new Session();
      var handledState = HandledState.ForUserSpecifiedSeverity(originalSeverity);

      var @event = new Event("1", app, device, exception, handledState, breadcrumbs, session);

      @event.Severity = updatedSeverity;

      Assert.Equal(updatedSeverity, @event.Severity);
      Assert.Contains("severityReason", @event.Keys);
    }
  }
}
