using Xunit;

namespace Bugsnag.ConfigurationSection.Tests
{
  public class DefaultTests
  {
    private readonly IConfiguration _testConfiguration;

    public DefaultTests()
    {
      _testConfiguration = Configuration.Settings;
    }

    private IConfiguration TestConfiguration => _testConfiguration;

    [Fact]
    public void ConfigurationIsNotNull()
    {
      Assert.NotNull(TestConfiguration);
    }

    [Fact]
    public void AppTypeIsNull()
    {
      Assert.Null(TestConfiguration.AppType);
    }

    [Fact]
    public void AppVersionIsNull()
    {
      Assert.Null(TestConfiguration.AppVersion);
    }

    [Fact]
    public void EndpointIsNotNull()
    {
      Assert.NotNull(TestConfiguration.Endpoint);
    }

    [Fact]
    public void NotifyReleaseStagesIsNull()
    {
      Assert.Null(TestConfiguration.NotifyReleaseStages);
    }

    [Fact]
    public void ReleaseStageIsNull()
    {
      Assert.Null(TestConfiguration.ReleaseStage);
    }

    [Fact]
    public void ProjectRootsIsNull()
    {
      Assert.Null(TestConfiguration.ProjectRoots);
    }

    [Fact]
    public void ProjectNamespacesIsNull()
    {
      Assert.Null(TestConfiguration.ProjectNamespaces);
    }

    [Fact]
    public void IgnoreClassesIsNull()
    {
      Assert.Null(TestConfiguration.IgnoreClasses);
    }

    [Fact]
    public void MetadataFiltersIsNull()
    {
      Assert.Null(TestConfiguration.MetadataFilters);
    }

    [Fact]
    public void GlobalMetadataIsNull()
    {
      Assert.Null(TestConfiguration.GlobalMetadata);
    }

    [Fact]
    public void AutoCaptureSessionsIsFalse()
    {
      Assert.False(TestConfiguration.AutoCaptureSessions);
    }

    [Fact]
    public void SessionsEndpointIsNotNull()
    {
      Assert.NotNull(TestConfiguration.SessionEndpoint);
    }

    [Fact]
    public void ProxyIsNull()
    {
      Assert.Null(TestConfiguration.Proxy);
    }

    [Fact]
    public void AutoNotifyIsTrue()
    {
      Assert.True(TestConfiguration.AutoNotify);
    }
  }
}
