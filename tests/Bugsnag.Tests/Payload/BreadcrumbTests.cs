using System;
using System.Collections.Generic;
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

      Assert.IsType<DateTime>(breadcrumb["timestamp"]);
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
      var report = new Report(configuration, new System.Exception(), Bugsnag.Payload.HandledState.ForHandledException(), new Breadcrumb[0], new Session(), new Request());
      var breadcrumb = Breadcrumb.FromReport(report);

      Assert.Equal("error", breadcrumb["type"]);
      Assert.NotNull(breadcrumb["metaData"]);
      Assert.IsType<DateTime>(breadcrumb["timestamp"]);
    }

    [Fact]
    public void NullNameProvidesDefault()
    {
      var breadcrumb = new Breadcrumb(null, BreadcrumbType.Manual);

      Assert.NotNull(breadcrumb.Name);
    }

    [Fact]
    public void LongNamesAreTrimmed()
    {
      var name = new String('a', 500);

      var breadcrumb = new Breadcrumb(name, BreadcrumbType.Manual);

      Assert.Equal(name.Substring(0, 30), breadcrumb.Name);
    }
  }
}
