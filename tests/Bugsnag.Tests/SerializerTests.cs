using System;
using System.Collections.Generic;
using Bugsnag.Payload;
using Xunit;

namespace Bugsnag.Tests
{
  public class SerializerTests
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

      var report = new Report(configuration, exception, Bugsnag.Payload.HandledState.ForHandledException(), new Breadcrumb[] { new Breadcrumb("test", BreadcrumbType.Manual) }, new Session());

      var json = Serializer.SerializeObject(report);
      Assert.NotNull(json);
    }

    [Fact]
    public void CircularReferenceTest()
    {
      var primary = new Dictionary<string, object>();
      var secondary = new Dictionary<string, object>() { { "primary", primary } };
      primary["secondary"] = secondary;
      var json = Serializer.SerializeObject(primary);
      Assert.Contains("[Circular]", json);
    }

    [Fact]
    public void OtherCircularReferenceTest()
    {
      var inner = new Circular { Name = "inner" };
      var outer = new Circular { Name = "outer", Inner = inner };
      inner.Inner = outer;

      var json = Serializer.SerializeObject(outer);

      Assert.Contains("[Circular]", json);
    }

    [Fact]
    public void NullValueTest()
    {
      var o = new Dictionary<string, object> { { "test", null } };
      var json = Serializer.SerializeObject(o);
      Assert.NotNull(json);
    }

    [Fact]
    public void CanSerializeException()
    {
      try
      {
        throw new System.Exception("Serialize me");
      }
      catch (System.Exception exception)
      {
        var json = Serializer.SerializeObject(exception);
        Assert.NotNull(json);
      }
    }

    private class Circular
    {
      public string Name { get; set; }

      public Circular Inner { get; set; }
    }

    public class FilterableTestObject : Dictionary<string, object>, IFilterable
    {

    }

    [Theory]
    [MemberData(nameof(FilterTestData))]
    public void FilterTests(object obj, string[] filters, int filteredCount)
    {
      var json = Serializer.SerializeObject(obj, filters);

      Assert.Equal(filteredCount, System.Text.RegularExpressions.Regex.Matches(json, "\\[Filtered\\]").Count);
    }

    public static IEnumerable<object[]> FilterTestData()
    {
      yield return new object[] { new Dictionary<string, object> { { "password", "password" } }, new string[] { "password" }, 0 };
      yield return new object[] { new Dictionary<string, object> { { "password", "password" }, { "additional", new FilterableTestObject { { "password", "password" } } } }, new string[] { "password" }, 1 };
      yield return new object[] { new FilterableTestObject { { "password", "password" } }, new string[] { "password" }, 1 };
      yield return new object[] { new FilterableTestObject { { "password", "password" } }, null, 0 };
      yield return new object[] { new FilterableTestObject { { "username", "password" } }, new string[] { "password" }, 0 };
      yield return new object[] { new FilterableTestObject { { "password", "password" }, { "credit_card_number", "number" } }, new string[] { "password", "credit_card_number" }, 2 };
      yield return new object[] { new FilterableTestObject { { "Password", "password" }, { "Credit_Card_Number", "number" } }, new string[] { "password", "credit_card_number" }, 2 };
    }
  }
}
