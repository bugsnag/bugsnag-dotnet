using Bugsnag.Payload;
using System.Collections.Generic;
using Xunit;

namespace Bugsnag.Tests
{
  public class MiddlewareTests
  {
    [Theory]
    [MemberData(nameof(ReleaseStageTestData))]
    public void ReleaseStageFilterTests(string releaseStage, string[] notifyReleaseStages, bool validReleaseStage)
    {
      var configuration = new Configuration("123456") { ReleaseStage = releaseStage, NotifyReleaseStages = notifyReleaseStages };

      var report = new Report(configuration, new System.Exception(), Bugsnag.Payload.HandledState.ForHandledException(), new Breadcrumb[0], new Session());

      InternalMiddleware.ReleaseStageFilter(report);

      Assert.Equal(validReleaseStage, report.Ignored);
    }

    public static IEnumerable<object[]> ReleaseStageTestData()
    {
      yield return new object[] { "production", new string[] { "production" }, false };
      yield return new object[] { "production", new string[] { "production", "test", "development" }, false };
      yield return new object[] { "test", new string[] { "production" }, true };
      yield return new object[] { "development", new string[] { "production", "test" }, true };
      yield return new object[] { null, new string[] { "production" }, false };
      yield return new object[] { null, null, false };
      yield return new object[] { "production", null, false };
    }

    [Theory]
    [MemberData(nameof(ProjectRootTestData))]
    public void ProjectRootStrippingTests(string[] projectRoots, string fileName, string expectedFileName)
    {
      var configuration = new Configuration("123456") { ProjectRoots = projectRoots };
      var report = new Report(configuration, new System.Exception(), HandledState.ForHandledException(), new Breadcrumb[0], new Session());

      foreach (var @event in report.Events)
      {
        foreach (var exception in @event.Exceptions)
        {
          var stacktrace = new StackTraceLine[] { new StackTraceLine(fileName, 1, string.Empty, false, null) };
          exception["stacktrace"] = stacktrace;
        }
      }

      InternalMiddleware.RemoveProjectRoots(report);

      foreach (var @event in report.Events)
      {
        foreach (var exception in @event.Exceptions)
        {
          foreach (var stacktraceline in exception.StackTrace)
          {
            Assert.Equal(expectedFileName, stacktraceline.FileName);
          }
        }
      }
    }

    public static IEnumerable<object[]> ProjectRootTestData()
    {
      yield return new object[] { new string[] { @"C:\app" }, @"C:\app\Class.cs", "Class.cs" };
      yield return new object[] { new string[] { @"C:\app\" }, @"C:\app\Class.cs", "Class.cs" };
    }
  }
}
