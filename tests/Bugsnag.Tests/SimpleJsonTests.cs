using System;
using System.Collections.Generic;
using Bugsnag.Payload;
using Xunit;

namespace Bugsnag.Tests
{
  public class SimpleJsonTests
  {
    [Fact]
    public void CanSerialiseReport()
    {
      System.Exception exception = null;
      var configuration = new Configuration("123456");

      try
      {
        throw new System.Exception("test");
      }
      catch (System.Exception caughtException)
      {
        exception = caughtException;
      }

      var report = new Report(configuration, exception, Bugsnag.Payload.HandledState.ForHandledException(), new Breadcrumb[] { new Breadcrumb("test", BreadcrumbType.Manual) }, new Session(), new Request());

      var json = SimpleJson.SimpleJson.SerializeObject(report);
      Assert.NotNull(json);
    }

    [Fact]
    public void CircularReferenceTest()
    {
      var primary = new Dictionary<string, object>();
      var secondary = new Dictionary<string, object>() { { "primary", primary } };
      primary["secondary"] = secondary;
      var json = SimpleJson.SimpleJson.SerializeObject(primary);
      Assert.Contains("[Circular]", json);
    }
  }
}
