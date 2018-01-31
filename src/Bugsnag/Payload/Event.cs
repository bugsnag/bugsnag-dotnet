using System.Collections.Generic;
using System.Linq;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Represents a single event in a Bugsnag payload.
  /// </summary>
  public class Event : Dictionary<string, object>
  {
    public Event(IConfiguration configuration, System.Exception exception, Severity severity, IEnumerable<Breadcrumb> breadcrumbs)
    {
      this["payloadVersion"] = 4;
      this["exceptions"] = new Exceptions(exception).ToArray();
      this["app"] = new App(configuration);
      this["device"] = new Device();
      this["metaData"] = new Metadata();
      this.AddToPayload("breadcrumbs", breadcrumbs);

      foreach (var item in severity)
      {
        this[item.Key] = item.Value;
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

    public Metadata Metadata
    {
      get { return this["metaData"] as Metadata; }
    }
  }
}
