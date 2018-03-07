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
    }

    [Fact]
    public void HandledExceptionPayloadIsCorrect()
    {
      var handledState = HandledState.ForHandledException();

      Assert.False((bool)handledState["unhandled"]);
      Assert.Equal("warning", handledState["severity"]);
    }

    [Fact]
    public void UserSpecifiedSeverityPayloadIsCorrect()
    {
      var handledState = HandledState.ForUserSpecifiedSeverity(Severity.Warning);

      Assert.False((bool)handledState["unhandled"]);
      Assert.Equal("warning", handledState["severity"]);
    }

    [Fact]
    public void CallbackSpecifiedSeverityPayloadIsCorrect()
    {
      var originalHandledState = HandledState.ForUnhandledException();
      var handledState = HandledState.ForCallbackSpecifiedSeverity(Severity.Info, originalHandledState);

      Assert.True((bool)handledState["unhandled"]); // same as the original severity
      Assert.Equal("info", handledState["severity"]);
    }
  }
}
