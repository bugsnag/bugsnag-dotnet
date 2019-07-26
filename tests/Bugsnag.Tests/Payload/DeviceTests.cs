using Bugsnag.Payload;
using System.Collections.Generic;
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

    [Fact]
    public void IncludesTime()
    {
      var device = new Device("hostname");
      Assert.NotNull(device["time"]);
    }

    [Fact]
    public void IncludesRuntimeVersions()
    {
      var device = new Device("hostname");
      var runtimeVersions = (Dictionary<string, string>) device["runtimeVersions"];
      Assert.NotNull(runtimeVersions["dotnet"]);
      Assert.NotNull(runtimeVersions["dotnetClr"]);
    }

    [Fact]
    public void Serializable()
    {
      var device = new Device("hostname");
      var p = Serializer.SerializeObject(device);
      Assert.NotNull(p);
    }
  }
}
