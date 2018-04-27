using Bugsnag.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bugsnag.Tests
{
  public class PayloadExtensionsTests
  {
    [Theory]
    [MemberData(nameof(PayloadTestData))]
    public void AddToPayloadTests(Dictionary<string, object> dictionary, string key, object value, object expectedValue)
    {
      dictionary.AddToPayload(key, value);

      if (expectedValue == null)
      {
        Assert.DoesNotContain(key, dictionary.Keys);
      }
      else
      {
        Assert.Equal(expectedValue, dictionary[key]);
      }
    }

    public static IEnumerable<object[]> PayloadTestData()
    {
      yield return new object[] { new Dictionary<string, object>(), "key", 1, 1 };
      yield return new object[] { new Dictionary<string, object>(), "key", null, null };
      yield return new object[] { new Dictionary<string, object>(), "key", "", null };
      yield return new object[] { new Dictionary<string, object>(), "key", "value", "value" };
      yield return new object[] { new Dictionary<string, object> { { "key", 1 } }, "key", null, null };
      yield return new object[] { new Dictionary<string, object> { { "key", 1 } }, "key", "", null };
    }

    [Fact]
    public void GetExistingKey()
    {
      var dictionary = new Dictionary<string, object> { { "key", "value" } };

      Assert.Equal("value", dictionary.Get("key"));
    }

    [Fact]
    public void GetNonExistentKeyReturnsNull()
    {
      var dictionary = new Dictionary<string, object>();

      Assert.Null(dictionary.Get("key"));
    }
  }
}
