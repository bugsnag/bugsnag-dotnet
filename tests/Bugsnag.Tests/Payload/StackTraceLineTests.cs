using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Bugsnag.Tests.Payload
{
  public class StackTraceLineTests
  {
    [Fact]
    public void HandleEmptyStackTrace()
    {
      var stackTrace = new Bugsnag.Payload.StackTrace(new Exception());

      Assert.Empty(stackTrace);
    }

    [Fact]
    public void BuildStackTrace()
    {
      Exception ex;

      try
      {
        throw new Exception();
      }
      catch (Exception e)
      {
        ex = e;
      }

      var stackTrace = new Bugsnag.Payload.StackTrace(ex);

      Assert.Equal(1, stackTrace.Count());

      var stackTraceLine = stackTrace.First();

      Assert.NotNull(stackTraceLine["file"]);
      Assert.NotNull(stackTraceLine["lineNumber"]);
      Assert.NotNull(stackTraceLine["method"]);
      Assert.NotNull(stackTraceLine["inProject"]);
    }
  }
}
