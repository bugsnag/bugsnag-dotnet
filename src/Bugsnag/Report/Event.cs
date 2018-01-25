using System.Collections.Generic;
using System.Linq;

namespace Bugsnag
{
  public class Event : Dictionary<string, object>
  {
    public Event(IConfiguration configuration, System.Exception exception, Severity severity)
    {
      this["payloadVersion"] = 4;
      this["exceptions"] = new Exceptions(exception).ToArray();
      this["app"] = new App(configuration);

      switch (severity)
      {
        case Severity.Info:
          this["severity"] = "info";
          break;
        case Severity.Warning:
          this["severity"] = "warning";
          break;
        default:
          this["severity"] = "error";
          break;
      }
    }

    public Exception[] Exceptions
    {
      get { return this["exceptions"] as Exception[]; }
      set { this.AddToPayload("exceptions", value); }
    }

    public string Context
    {
      get { return this["context"] as string; }
      set { this.AddToPayload("context", value); }
    }

    public string GroupingHash
    {
      get { return this["groupingHash"] as string; }
      set { this.AddToPayload("groupingHash", value); }
    }
  }
}
