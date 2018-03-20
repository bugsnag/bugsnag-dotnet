using Bugsnag.Payload;
using System;
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

    [Theory]
    [MemberData(nameof(DetermineDefaultContextTestData))]
    public void DetermineDefaultContextTests(string requestUrl, string expectedContext)
    {
      var configuration = new Configuration("123456");
      var report = new Report(configuration, new System.Exception(), HandledState.ForHandledException(), new Breadcrumb[0], new Session());
      foreach (var @event in report.Events)
      {
        @event.Request = new Request { Url = requestUrl };
      }

      InternalMiddleware.DetermineDefaultContext(report);

      foreach (var @event in report.Events)
      {
        Assert.Equal(expectedContext, @event.Context);
      }
    }

    public static IEnumerable<object[]> DetermineDefaultContextTestData()
    {
      yield return new object[] { "not-a-valid-url", "not-a-valid-url" };
      yield return new object[] { "https://app.bugsnag.com/user/new/", "/user/new/" };
      yield return new object[] { "https://app.bugsnag.com/user/new/?query=ignored", "/user/new/" };
      yield return new object[] { null, null };
    }

    [Theory]
    [MemberData(nameof(IgnoreClassesTestData))]
    public void IgnoreClassesTest(System.Exception thrownException, Type ignoreClass, bool ignored)
    {
      var configuration = new Configuration("123456") { IgnoreClasses = new[] { ignoreClass } };
      var report = new Report(configuration, thrownException, HandledState.ForHandledException(), new Breadcrumb[0], new Session());

      InternalMiddleware.RemoveIgnoredExceptions(report);

      Assert.Equal(ignored, report.Ignored);
    }

    public static IEnumerable<object[]> IgnoreClassesTestData()
    {
      yield return new object[] { new System.Exception(), typeof(System.Exception), true };
      yield return new object[] { new System.DllNotFoundException(), typeof(System.Exception), true };
      yield return new object[] { new System.Exception(), typeof(System.DllNotFoundException), false };
    }
  }
}
