using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Xunit;

namespace Bugsnag.ConfigurationSection.Tests
{
  public class CompleteTests
  {
    private readonly Configuration _completeConfiguration;

    public CompleteTests()
    {
      ConfigurationFileMap fileMap = new ConfigurationFileMap(".\\Complete.config");
      _completeConfiguration = ConfigurationManager.OpenMappedMachineConfiguration(fileMap).GetSection("bugsnag") as Configuration;
    }

    private Configuration CompleteConfiguration => _completeConfiguration;

    [Fact]
    public void ConfigurationIsNotNull()
    {
      Assert.NotNull(CompleteConfiguration);
    }

    [Fact]
    public void AppTypeIsSet()
    {
      Assert.Equal("test", CompleteConfiguration.AppType);
    }

    [Fact]
    public void AppVersionIsSet()
    {
      Assert.Equal("1.0", CompleteConfiguration.AppVersion);
    }

    [Fact]
    public void EndpointIsSet()
    {
      Assert.Equal(new Uri("https://www.bugsnag.com"), CompleteConfiguration.Endpoint);
    }

    [Fact]
    public void NotifyReleaseStagesIsSet()
    {
      Assert.Equal(new[] { "development", "test", "production" }, CompleteConfiguration.NotifyReleaseStages);
    }

    [Fact]
    public void ReleaseStageIsSet()
    {
      Assert.Equal("test", CompleteConfiguration.ReleaseStage);
    }

    [Fact]
    public void ProjectRootsIsSet()
    {
      Assert.Equal(new[] { @"C:\app", @"D:\src" }, CompleteConfiguration.ProjectRoots);
    }

    [Fact]
    public void ProjectNamespacesIsSet()
    {
      Assert.Equal(new[] { "App.Code", "Bugsnag.Tests" }, CompleteConfiguration.ProjectNamespaces);
    }

    [Fact]
    public void IgnoreClassesIsSet()
    {
      Assert.Equal(new[] { "NotAGoodClass", "NotThatBadException" }, CompleteConfiguration.IgnoreClasses);
    }

    [Fact]
    public void MetadataFiltersIsSet()
    {
      Assert.Equal(new[] { "password", "creditcard" }, CompleteConfiguration.MetadataFilters);
    }

    [Fact]
    public void GlobalMetadataIsSet()
    {
      Assert.Equal(new[] { new KeyValuePair<string, string>("test", "wow") }, CompleteConfiguration.GlobalMetadata);
    }

    [Fact]
    public void TrackSessionsIsFalse()
    {
      Assert.False(CompleteConfiguration.TrackSessions);
    }

    [Fact]
    public void SessionsEndpointIsSet()
    {
      Assert.Equal(new Uri("https://www.bugsnag.com"), CompleteConfiguration.SessionEndpoint);
    }
  }
}
