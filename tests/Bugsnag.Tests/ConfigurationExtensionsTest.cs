using System.Collections.Generic;
using Xunit;

namespace Bugsnag.Tests
{
  public class ConfigurationExtensionsTest
  {
    [Theory]
    [MemberData(nameof(TestData))]
    public void Test(string releaseStage, string[] notifyReleaseStages, bool validReleaseStage)
    {
      var configuration = new Configuration("123456") { ReleaseStage = releaseStage, NotifyReleaseStages = notifyReleaseStages };

      Assert.Equal(validReleaseStage, configuration.ValidReleaseStage());
    }

    public static IEnumerable<object[]> TestData()
    {
      yield return new object[] { "production", new string[] { "production" }, true };
      yield return new object[] { "production", new string[] { "production", "test", "development" }, true };
      yield return new object[] { "test", new string[] { "production" }, false };
      yield return new object[] { "development", new string[] { "production", "test" }, false };
      yield return new object[] { null, new string[] { "production" }, false };
      yield return new object[] { null, null, true };
      yield return new object[] { "production", null, true };
    }
  }
}
