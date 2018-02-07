using System.Collections.Generic;

namespace Bugsnag.Payload
{
  public class NotifierInfo : Dictionary<string, string>
  {
    /// <summary>
    /// A single instance of the current notifier info to attach to all error reports.
    /// </summary>
    private static NotifierInfo NotifierInfoInstance = new NotifierInfo {
      { "name", ".NET Bugsnag Notifier" },
      { "version", typeof(Client).GetAssembly().GetName().Version.ToString(3) },
      { "url", "https://github.com/bugsnag/bugsnag-net" }
    };

    public static NotifierInfo Instance
    {
      get
      {
        return NotifierInfoInstance;
      }
    }
  }
}
