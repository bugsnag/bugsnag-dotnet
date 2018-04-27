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

    [Theory]
    [MemberData(nameof(ContextIsSetByRequestData))]
    public void ContextIsSetByRequest(Request request, string existingContext, string expectedContext)
    {
      var configuration = new Configuration("123456");
      var report = new Report(configuration, new System.Exception(), HandledState.ForHandledException(), new Breadcrumb[0], new Session());
      report.Event.Context = existingContext;
      report.Event.Request = request;

      Assert.Equal(expectedContext, report.Event.Context);
    }

    public static IEnumerable<object[]> ContextIsSetByRequestData()
    {
      yield return new object[] { new Request { Url = "not-a-valid-url" }, null, "not-a-valid-url" };
      yield return new object[] { new Request { Url = "https://app.bugsnag.com/user/new/" }, null, "/user/new/" };
      yield return new object[] { new Request { Url = "https://app.bugsnag.com/user/new/?query=ignored" }, null, "/user/new/" };
      yield return new object[] { null, null, null };
      yield return new object[] { new Request { Url = "not-a-valid-url" }, "existing-context", "existing-context" };
      yield return new object[] { new Request { Url = "https://app.bugsnag.com/user/new/" }, "existing-context", "existing-context" };
      yield return new object[] { new Request { Url = "https://app.bugsnag.com/user/new/?query=ignored" }, "existing-context", "existing-context" };
      yield return new object[] { null, "existing-context", "existing-context" };
    }
  }
}
