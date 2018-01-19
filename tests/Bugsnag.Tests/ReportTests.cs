using System;
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

      var report = new Report(configuration, exception, Severity.Error);
      Assert.NotNull(report);
    }

    private class TestConfiguration : IConfiguration
    {
      public string ApiKey => "123456";

      public Uri Endpoint => new Uri("https://notify.bugsnag.com");

      public string ReleaseStage => "test";

      public string[] NotifyReleaseStages => null;

      public string AppVersion => "1.0";

      public string AppType => "test";
    }
  }
}
