using Xunit;

namespace Bugsnag.Tests.Payload
{
  public class SeverityTests
  {
    [Fact]
    public void UnhandledExceptionPayloadIsCorrect()
    {
      var severity = Bugsnag.Payload.Severity.ForUnhandledException();

      Assert.True((bool)severity["unhandled"]);
      Assert.Equal("error", severity["severity"]);
    }

    [Fact]
    public void HandledExceptionPayloadIsCorrect()
    {
      var severity = Bugsnag.Payload.Severity.ForHandledException();

      Assert.False((bool)severity["unhandled"]);
      Assert.Equal("warning", severity["severity"]);
    }

    [Fact]
    public void UserSpecifiedSeverityPayloadIsCorrect()
    {
      var severity = Bugsnag.Payload.Severity.ForUserSpecifiedSeverity(Severity.Warning);

      Assert.False((bool)severity["unhandled"]);
      Assert.Equal("warning", severity["severity"]);
    }

    [Fact]
    public void CallbackSpecifiedSeverityPayloadIsCorrect()
    {
      var originalSeverity = Bugsnag.Payload.Severity.ForUnhandledException();
      var severity = Bugsnag.Payload.Severity.ForCallbackSpecifiedSeverity(Severity.Info, originalSeverity);

      Assert.True((bool)severity["unhandled"]); // same as the original severity
      Assert.Equal("info", severity["severity"]);
    }
  }
}
