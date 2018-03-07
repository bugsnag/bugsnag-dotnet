using Bugsnag.Payload;
using System.Collections.Generic;
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

      var report = new Report(configuration, new System.Exception(), Bugsnag.Payload.Severity.ForHandledException(), new Breadcrumb[0], new Session(), new Request());

      InternalMiddleware.ReleaseStageFilter(report);

      Assert.Equal(validReleaseStage, report.Ignored);
    }

    public static IEnumerable<object[]> TestData()
    {
      yield return new object[] { "production", new string[] { "production" }, false };
      yield return new object[] { "production", new string[] { "production", "test", "development" }, false };
      yield return new object[] { "test", new string[] { "production" }, true };
      yield return new object[] { "development", new string[] { "production", "test" }, true };
      yield return new object[] { null, new string[] { "production" }, false };
      yield return new object[] { null, null, false };
      yield return new object[] { "production", null, false };
    }
  }
}
