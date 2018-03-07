using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bugsnag.ConfigurationSection.Tests
{
  public class CompleteTests
  {
    private readonly IConfiguration _testConfiguration;

    public CompleteTests()
    {
      ConfigurationFileMap fileMap = new ConfigurationFileMap(".\\Complete.config");
      _testConfiguration = ConfigurationManager.OpenMappedMachineConfiguration(fileMap).GetSection("bugsnag") as Configuration;
    }

    private IConfiguration TestConfiguration => _testConfiguration;

    [Fact]
    public void ConfigurationIsNotNull()
    {
      Assert.NotNull(TestConfiguration);
    }

    [Fact]
    public void AppTypeIsSet()
    {
      Assert.Equal("test", TestConfiguration.AppType);
    }

    [Fact]
    public void AppVersionIsSet()
    {
      Assert.Equal("1.0", TestConfiguration.AppVersion);
    }

    [Fact]
    public void EndpointIsSet()
    {
      Assert.Equal(new Uri("https://www.bugsnag.com"), TestConfiguration.Endpoint);
    }

    [Fact]
    public void NotifyReleaseStagesIsSet()
    {
      Assert.Equal(new[] { "development", "test", "production" }, TestConfiguration.NotifyReleaseStages);
    }

    [Fact]
    public void ReleaseStageIsSet()
    {
      Assert.Equal("test", TestConfiguration.ReleaseStage);
    }

    [Fact]
    public void ProjectRootsIsSet()
    {
      Assert.Equal(new[] { @"C:\app", @"D:\src" }, TestConfiguration.ProjectRoots);
    }

    [Fact]
    public void ProjectNamespacesIsSet()
    {
      Assert.Equal(new[] { "App.Code", "Bugsnag.Tests" }, TestConfiguration.ProjectNamespaces);
    }

    [Fact]
    public void IgnoreClassesIsSet()
    {
      Assert.Equal(new[] { "NotAGoodClass", "NotThatBadException" }, TestConfiguration.IgnoreClasses);
    }

    [Fact]
    public void MetadataFiltersIsSet()
    {
      Assert.Equal(new[] { "password", "creditcard" }, TestConfiguration.MetadataFilters);
    }

    [Fact]
    public void GlobalMetadataIsSet()
    {
      Assert.Equal(new[] { new KeyValuePair<string, string>("test", "wow") }, TestConfiguration.GlobalMetadata);
    }

    [Fact]
    public void AutoCaptureSessionsIsTrue()
    {
      Assert.True(TestConfiguration.AutoCaptureSessions);
    }

    [Fact]
    public void SessionsEndpointIsSet()
    {
      Assert.Equal(new Uri("https://www.bugsnag.com"), TestConfiguration.SessionEndpoint);
    }

    [Fact]
    public void AutoNotifyIsFalse()
    {
      Assert.False(TestConfiguration.AutoNotify);
    }

    [Fact]
    public void ProxyIsNotNull()
    {
      Assert.NotNull(TestConfiguration.Proxy);
    }

    [Fact]
    public void MaximumBreadcrumbsIsSet()
    {
      Assert.Equal(30, TestConfiguration.MaximumBreadcrumbs);
    }
  }
}
