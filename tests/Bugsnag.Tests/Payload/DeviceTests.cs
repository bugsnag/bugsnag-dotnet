using Bugsnag.Payload;
using Xunit;

namespace Bugsnag.Tests.Payload
{
  public class DeviceTests
  {
    [Fact]
    public void IncludesHostNameKey()
    {
      var device = new Device();
      Assert.NotNull(device["hostname"]);
    }

    [Fact]
    public void SpecifyHostName()
    {
      var hostname = "test";
      var device = new Device(hostname);
      Assert.Equal(hostname, device["hostname"]);
    }
  }
}
