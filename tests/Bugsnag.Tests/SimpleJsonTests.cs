using Xunit;

namespace Bugsnag.Tests
{
  public class SimpleJsonTests
  {
    [Fact]
    public void CanSerialiseReport()
    {
      System.Exception exception = null;
      var configuration = new TestConfiguration();

      try
      {
        throw new System.Exception("test");
      }
      catch (System.Exception caughtException)
      {
        exception = caughtException;
      }

      var report = new Report(configuration, exception, Severity.Error);
      var json = SimpleJson.SimpleJson.SerializeObject(report);
      Assert.NotNull(json);
    }
  }
}
