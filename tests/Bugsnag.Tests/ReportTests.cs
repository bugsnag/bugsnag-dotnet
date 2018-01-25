using System.Collections.Generic;
using Xunit;

namespace Bugsnag.Tests
{
  public class ReportTests
  {
    [Fact]
    public void BasicTest()
    {
      System.Exception exception = null;
      var configuration = new TestConfiguration();

      try
      {
        throw new System.Exception("test");
      }
      catch (System.Exception caughtException)
      {
        exception = caughtException;
      }

      var report = new Report(configuration, exception, Severity.Error, new List<Breadcrumb> { new Breadcrumb("test", BreadcrumbType.Manual) });
      Assert.NotNull(report);
    }
  }
}
