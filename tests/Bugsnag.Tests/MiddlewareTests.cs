using Bugsnag.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bugsnag.Tests
{
  public class MiddlewareTests
  {
    [Theory]
    [MemberData(nameof(TestData))]
    public void ReleaseStageFilterTests(string releaseStage, string[] notifyReleaseStages, bool validReleaseStage)
    {
      var configuration = new Configuration("123456") { ReleaseStage = releaseStage, NotifyReleaseStages = notifyReleaseStages };

      var report = new Report(configuration, new System.Exception(), Bugsnag.Payload.Severity.ForHandledException(), Enumerable.Empty<Breadcrumb>());

      InternalMiddleware.ReleaseStageFilter(configuration, report);

      Assert.Equal(validReleaseStage, report.Deliver);
    }

    public static IEnumerable<object[]> TestData()
    {
      yield return new object[] { "production", new string[] { "production" }, true };
      yield return new object[] { "production", new string[] { "production", "test", "development" }, true };
      yield return new object[] { "test", new string[] { "production" }, false };
      yield return new object[] { "development", new string[] { "production", "test" }, false };
      yield return new object[] { null, new string[] { "production" }, true };
      yield return new object[] { null, null, true };
      yield return new object[] { "production", null, true };
    }
  }
}
