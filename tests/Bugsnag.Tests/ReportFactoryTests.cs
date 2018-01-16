using System;
using Xunit;

namespace Bugsnag.Tests
{
  public class ReportFactoryTests
  {
    [Fact]
    public void BasicTest()
    {
      var configuration = new TestConfiguration();
      var payloadGenerator = new ReportFactory(configuration);

      try
      {
        throw new System.Exception("test");
      }
      catch (System.Exception exception)
      {
        var report = payloadGenerator.Generate(exception, Severity.Error);
        Assert.NotNull(report);
      }
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
