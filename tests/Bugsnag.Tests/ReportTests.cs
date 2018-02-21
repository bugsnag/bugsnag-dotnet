using Bugsnag.Payload;
using Xunit;

namespace Bugsnag.Tests
{
  public class ReportTests
  {
    [Fact]
    public void BasicTest()
    {
      System.Exception exception = null;
      var configuration = new Configuration("123456");

      try
      {
        throw new System.Exception("test");
      }
      catch (System.Exception caughtException)
      {
        exception = caughtException;
      }

      var report = new Report(configuration, exception, Bugsnag.Payload.Severity.ForHandledException(), new Breadcrumb[] { new Breadcrumb("test", BreadcrumbType.Manual) }, new Session(), new Request());
      Assert.NotNull(report);
    }
  }
}
