using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Represents the "device" key in the error report payload.
  /// </summary>
  public class Device : Dictionary<string, object>, IFilterable
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
      this.AddToPayload("runtimeVersions", RuntimeVersions);
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

    private static Dictionary<string, string> RuntimeVersions
    {
      get
      {
        var runtimeVersions = new Dictionary<string, string>();
#if NET45
        // The framework description is only available from .NET 4.5 onwards and will not return
        // an accurate value for .NET Core until version 3.0 
        runtimeVersions.AddToPayload("dotnet", RuntimeInformation.FrameworkDescription);
#endif
#if !NETSTANDARD1_3
        // The CLR version is available for all .NET framework versions and all .NET Core versions since 2.0
        runtimeVersions.AddToPayload("dotnetClr", Environment.Version.ToString());
#endif
        return runtimeVersions;
      }
    }
  }
}
