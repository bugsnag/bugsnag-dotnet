using System;
using System.Linq;
using Bugsnag.Payload;
using Xunit;

namespace Bugsnag.Tests.Payload
{
  public class BreadcrumbTests
  {
    [Fact]
    public void BreadcrumbIncludesTimestamp()
    {
      var breadcrumb = new Breadcrumb("test", BreadcrumbType.Manual);

      Assert.IsType(typeof(DateTime), breadcrumb["timestamp"]);
    }

    [Fact]
    public void BreadcrumbWithNoMetadataDoesNotIncludeKey()
    {
      var breadcrumb = new Breadcrumb("test", BreadcrumbType.Manual);

      Assert.DoesNotContain("metaData", breadcrumb.Keys);
    }

    [Fact]
    public void FromReportIncludesExpectedKeys()
    {
      var configuration = new Configuration("123456");
      var report = new Report(configuration, new System.Exception(), Bugsnag.Payload.Severity.ForHandledException(), Enumerable.Empty<Breadcrumb>(), new Session());
      var breadcrumb = Breadcrumb.FromReport(report);

      Assert.Equal("error", breadcrumb["type"]);
      Assert.NotNull(breadcrumb["metaData"]);
      Assert.IsType(typeof(DateTime), breadcrumb["timestamp"]);
    }
  }
}
