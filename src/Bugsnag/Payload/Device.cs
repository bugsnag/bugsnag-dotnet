using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

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
      var model = Model;
      if (model != null)
        this.AddToPayload("model", model);
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

    private static string Model
    {
      get
      {
#if NET6_0_OR_GREATER
        if (OperatingSystem.IsIOS() || OperatingSystem.IsMacCatalyst() || OperatingSystem.IsTvOS())
        {
          try
          {
            var uiDeviceType = Type.GetType("UIKit.UIDevice, Microsoft.iOS");
            if (uiDeviceType != null)
            {
              var currentDevice = uiDeviceType.GetProperty("CurrentDevice", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
              if (currentDevice != null)
                return uiDeviceType.GetProperty("Model", BindingFlags.Public | BindingFlags.Instance)?.GetValue(currentDevice) as string;
            }
          }
          catch
          {
            // Reflection may fail in trimmed/AOT builds
          }
        }
        else if (OperatingSystem.IsAndroid())
        {
          try
          {
            var buildType = Type.GetType("Android.OS.Build, Mono.Android");
            if (buildType != null)
              return buildType.GetProperty("Model", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string;
          }
          catch
          {
            // Reflection may fail in trimmed/AOT builds
          }
        }
#endif
        return null;
      }
    }

    private static string OsName
    {
      get
      {
#if NET6_0_OR_GREATER
        var version = Environment.OSVersion.Version;
        string name;
        if (OperatingSystem.IsIOS())
          name = "iOS";
        else if (OperatingSystem.IsMacCatalyst())
          name = "macOS (Mac Catalyst)";
        else if (OperatingSystem.IsTvOS())
          name = "tvOS";
        else if (OperatingSystem.IsWatchOS())
          name = "watchOS";
        else if (OperatingSystem.IsAndroid())
          name = "Android";
        else if (OperatingSystem.IsMacOS())
          name = "macOS";
        else if (OperatingSystem.IsWindows())
          name = "Windows";
        else if (OperatingSystem.IsLinux())
          name = "Linux";
        else
          return System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        return $"{name} {version}";
#elif NETSTANDARD2_0
        return System.Runtime.InteropServices.RuntimeInformation.OSDescription;
#else
        return Environment.OSVersion.VersionString;
#endif
      }
    }
  }
}
