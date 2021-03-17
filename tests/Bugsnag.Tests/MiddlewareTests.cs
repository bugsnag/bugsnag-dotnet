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

      foreach (var exception in report.Event.Exceptions)
      {
        var stacktrace = new StackTraceLine[] { new StackTraceLine(fileName, 1, string.Empty, false) };
        exception["stacktrace"] = stacktrace;
      }

      InternalMiddleware.RemoveProjectRoots(report);

      foreach (var exception in report.Event.Exceptions)
      {
        foreach (var stacktraceline in exception.StackTrace)
        {
          Assert.Equal(expectedFileName, stacktraceline.FileName);
        }
      }
    }

    public static IEnumerable<object[]> ProjectRootTestData()
    {
      yield return new object[] { new string[] { @"C:\app" }, @"C:\app\Class.cs", "Class.cs" };
      yield return new object[] { new string[] { @"C:\app\" }, @"C:\app\Class.cs", "Class.cs" };
      // for this scenario we should only strip the file path once, here we
      // have a setup where the first prefix will then also cause the second
      // prefix to match. This should only strip the first prefix
      yield return new object[] { new string[] { @"C:\app\", @"test\path" }, @"C:\app\test\path\Class.cs", @"test\path\Class.cs" };
    }

    [Theory]
    [MemberData(nameof(IgnoreClassesTestData))]
    public void IgnoreClassesTest(System.Exception thrownException, Type ignoreClass, bool ignored)
    {
      var configuration = new Configuration("123456") { IgnoreClasses = new[] { ignoreClass } };
      var report = new Report(configuration, thrownException, HandledState.ForHandledException(), new Breadcrumb[0], new Session());

      InternalMiddleware.CheckIgnoreClasses(report);

      Assert.Equal(ignored, report.Ignored);
    }

    public static IEnumerable<object[]> IgnoreClassesTestData()
    {
      yield return new object[] { new System.Exception(), typeof(System.Exception), true };
      yield return new object[] { new System.DllNotFoundException(), typeof(System.Exception), true };
      yield return new object[] { new System.Exception(), typeof(System.DllNotFoundException), false };
    }

    [Theory]
    [MemberData(nameof(InProjectTestData))]
    public void InProjectTest(string[] projectNamespaces, StackTraceInProjectTestCase[] testCases)
    {
      var configuration = new Configuration("123456") { ProjectNamespaces = projectNamespaces };
      var report = new Report(configuration, new System.Exception(), HandledState.ForHandledException(), new Breadcrumb[0], new Session());

      foreach (var exception in report.Event.Exceptions)
      {
        exception["stacktrace"] = testCases
          .Select(t => new StackTraceLine(null, 0, t.MethodName, false))
          .ToArray();
      }

      InternalMiddleware.DetectInProjectNamespaces(report);

      foreach (var exception in report.Event.Exceptions)
      {
        foreach (var stackTraceLine in exception.StackTrace)
        {
          foreach (var testCase in testCases)
          {
            if (stackTraceLine.MethodName == testCase.MethodName)
            {
              Assert.Equal(testCase.ShouldBeMarkedAsInProject, stackTraceLine.InProject);
            }
          }
        }
      }
    }

    public class StackTraceInProjectTestCase
    {
      public string MethodName { get; set; }

      public bool ShouldBeMarkedAsInProject { get; set; }
    }

    public static IEnumerable<object[]> InProjectTestData()
    {
      yield return new object[] {
        null,
        new StackTraceInProjectTestCase[] { }
      };
      yield return new object[] {
        new string[0],
        new StackTraceInProjectTestCase[] { }
      };
      yield return new object[] {
        new string[] { "Bugsnag.Code" },
        new StackTraceInProjectTestCase[] {
          new StackTraceInProjectTestCase {
            MethodName = "Bugsnag.Code.Wow",
            ShouldBeMarkedAsInProject = true }
        }
      };
      yield return new object[] {
        new string[] { "Bugsnag.Code" },
        new StackTraceInProjectTestCase[] {
          new StackTraceInProjectTestCase {
            MethodName = "Bugsnag.Assets",
            ShouldBeMarkedAsInProject = false }
        }
      };
      yield return new object[] {
        new string[] { "Bugsnag.Code", "Bugsnag.Assets" },
        new StackTraceInProjectTestCase[] {
          new StackTraceInProjectTestCase {
            MethodName = "Bugsnag.Code.Wow",
            ShouldBeMarkedAsInProject = true }
        }
      };
    }
  }
}
