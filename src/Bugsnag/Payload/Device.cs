using System;
using System.Collections.Generic;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Represents the "device" key in the error report payload.
  /// </summary>
  public class Device : Dictionary<string, string>
  {
    public Device() : this(Hostname)
    {
    }

    public Device(string hostname)
    {
      this.AddToPayload("hostname", hostname);
    }

    /// <summary>
    /// Resolve the hostname using either "COMPUTERNAME" (win) or "HOSTNAME" (*nix) environment variable.
    /// </summary>
    private static string Hostname
    {
      get
      {
        return Environment.GetEnvironmentVariable("COMPUTERNAME") ?? Environment.GetEnvironmentVariable("HOSTNAME");
      }
    }
  }
}
