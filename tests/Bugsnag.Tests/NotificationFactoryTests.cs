using System;
using Xunit;

namespace Bugsnag.Tests
{
  public class NotificationFactoryTests
  {
    [Fact]
    public void BasicTest()
    {
      var configuration = new TestConfiguration();
      var payloadGenerator = new NotificationFactory(configuration);

      try
      {
        throw new Exception("test");
      }
      catch (Exception exception)
      {
        var notification = payloadGenerator.Generate(exception, Severity.Error);
        Assert.NotNull(notification);
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
