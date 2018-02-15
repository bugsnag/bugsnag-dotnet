using Xunit;

namespace Bugsnag.ConfigurationSection.Tests
{
  public class DefaultTests
  {
    [Fact]
    public void ConfigurationIsNotNull()
    {
      Assert.NotNull(Configuration.Settings);
    }

    [Fact]
    public void AppTypeIsNull()
    {
      Assert.Null(Configuration.Settings.AppType);
    }

    [Fact]
    public void AppVersionIsNull()
    {
      Assert.Null(Configuration.Settings.AppVersion);
    }

    [Fact]
    public void EndpointIsNotNull()
    {
      Assert.NotNull(Configuration.Settings.Endpoint);
    }

    [Fact]
    public void NotifyReleaseStagesIsNull()
    {
      Assert.Null(Configuration.Settings.NotifyReleaseStages);
    }

    [Fact]
    public void ReleaseStageIsNull()
    {
      Assert.Null(Configuration.Settings.ReleaseStage);
    }

    [Fact]
    public void ProjectRootsIsNull()
    {
      Assert.Null(Configuration.Settings.ProjectRoots);
    }

    [Fact]
    public void ProjectNamespacesIsNull()
    {
      Assert.Null(Configuration.Settings.ProjectNamespaces);
    }

    [Fact]
    public void IgnoreClassesIsNull()
    {
      Assert.Null(Configuration.Settings.IgnoreClasses);
    }

    [Fact]
    public void MetadataFiltersIsNull()
    {
      Assert.Null(Configuration.Settings.MetadataFilters);
    }

    [Fact]
    public void GlobalMetadataIsNull()
    {
      Assert.Null(Configuration.Settings.GlobalMetadata);
    }

    [Fact]
    public void TrackSessionsIsTrue()
    {
      Assert.True(Configuration.Settings.TrackSessions);
    }

    [Fact]
    public void SessionsEndpointIsNotNull()
    {
      Assert.NotNull(Configuration.Settings.SessionEndpoint);
    }
  }
}
