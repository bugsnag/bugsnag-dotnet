using Xunit;
using Bugsnag.Payload;

namespace Bugsnag.Tests.Payload
{
  public class HandledStateTests
  {
    [Fact]
    public void UnhandledExceptionPayloadIsCorrect()
    {
      var handledState = HandledState.ForUnhandledException();

      Assert.True((bool)handledState["unhandled"]);
      Assert.Equal("error", handledState["severity"]);
      Assert.Equal(Severity.Error, handledState.Severity);
    }

    [Fact]
    public void HandledExceptionPayloadIsCorrect()
    {
      var handledState = HandledState.ForHandledException();

      Assert.False((bool)handledState["unhandled"]);
      Assert.Equal("warning", handledState["severity"]);
      Assert.Equal(Severity.Warning, handledState.Severity);
    }

    [Fact]
    public void UserSpecifiedSeverityPayloadIsCorrect()
    {
      const Severity warning = Severity.Warning;
      var handledState = HandledState.ForUserSpecifiedSeverity(warning);

      Assert.False((bool)handledState["unhandled"]);
      Assert.Equal("warning", handledState["severity"]);
      Assert.Equal(warning, handledState.Severity);
    }

    [Fact]
    public void CallbackSpecifiedSeverityPayloadIsCorrect()
    {
      var originalHandledState = HandledState.ForUnhandledException();
      const Severity info = Severity.Info;
      var handledState = HandledState.ForCallbackSpecifiedSeverity(info, originalHandledState);

      Assert.True((bool)handledState["unhandled"]); // same as the original severity
      Assert.Equal("info", handledState["severity"]);
      Assert.Equal(info, handledState.Severity);
    }
  }
}
