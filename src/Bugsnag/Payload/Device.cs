using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Represents the "device" key in the error report payload.
  /// </summary>
  public class Device : Dictionary<string, object>
  {
    public Device() : this(Hostname)
    {
    }

    public Device(string hostname)
    {
      this.AddToPayload("hostname", hostname);
      this.AddToPayload("locale", CultureInfo.CurrentCulture.ToString());
      this.AddToPayload("timezone", TimeZoneInfo.Local.DisplayName);
      this.AddToPayload("osName", OsName);
      this.AddToPayload("time", DateTime.UtcNow);
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

    private static string OsName
    {
      get
      {
#if NETSTANDARD1_3 || NETSTANDARD2_0
        return System.Runtime.InteropServices.RuntimeInformation.OSDescription;
#else
        return Environment.OSVersion.VersionString;
#endif
      }
    }
  }
}
