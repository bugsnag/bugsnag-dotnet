using System;
using System.Collections.Generic;

namespace Bugsnag.Payload
{
  public class Device : Dictionary<string, string>
  {
    public Device() : this(Hostname)
    {
    }

    public Device(string hostname)
    {
      this.AddToPayload("hostname", hostname);
    }

    private static string Hostname
    {
      get
      {
        return Environment.GetEnvironmentVariable("COMPUTERNAME") ?? Environment.GetEnvironmentVariable("HOSTNAME");
      }
    }
  }
}
