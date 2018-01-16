using Xunit;

namespace Bugsnag.Tests
{
  public class SimpleJsonTests
  {
    [Fact]
    public void CanSerialiseReport()
    {
      var stackTraceLine = new StackTraceLine("IBroken.cs", 5, "public void Boop()", true);
      var exception = new Exception("Exception", "Oh no an exception", new[] { stackTraceLine });
      var app = new App("1.0", "test", "client");
      var @event = new Event(new[] { exception }, Severity.Error, app);
      var report = new Report("123abc", @event);
      var json = SimpleJson.SimpleJson.SerializeObject(report);
      Assert.NotNull(json);
    }
  }
}
