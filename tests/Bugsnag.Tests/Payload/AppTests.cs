using Xunit;

namespace Bugsnag.Tests.Payload
{
  public class AppTests
  {
    [Fact]
    public void BuildAppFromConfigurationSetsCorrectValues()
    {
      var configuration = new Configuration("123456") { AppVersion = "1.0", ReleaseStage = "production", AppType = "web" };
      var app = new Bugsnag.Payload.App(configuration);

      Assert.Equal(configuration.AppVersion, app["version"]);
      Assert.Equal(configuration.ReleaseStage, app["releaseStage"]);
      Assert.Equal(configuration.AppType, app["type"]);
    }
  }
}
