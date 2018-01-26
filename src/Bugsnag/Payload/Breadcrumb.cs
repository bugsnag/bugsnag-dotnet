using System;
using System.Collections.Generic;

namespace Bugsnag.Payload
{
  public class Breadcrumb : Dictionary<string, object>
  {
    public static Breadcrumb FromReport(Report report)
    {
      var name = report.OriginalException.GetType().ToString();
      var type = BreadcrumbType.Error;
      var metadata = new Dictionary<string, string>
      {
        { "message", report.OriginalException.Message },
        { "severity", report.OriginalSeverity.ToString() },
      };

      return new Breadcrumb(name, type, metadata);
    }

    public Breadcrumb(string name, BreadcrumbType type) : this(name, type, null)
    {

    }

    public Breadcrumb(string name, BreadcrumbType type, IDictionary<string, string> metadata)
    {
      this["name"] = name; // limit this to 30 characters? provide a default incase of null or empty?
      this["timestamp"] = DateTime.UtcNow;
      this.AddToPayload("metaData", metadata); // can we limit the size of this somehow? Should it be <string, object>?

      switch (type)
      {
        case BreadcrumbType.Navigation:
          this["type"] = "navigation";
          break;
        case BreadcrumbType.Request:
          this["type"] = "request";
          break;
        case BreadcrumbType.Process:
          this["type"] = "process";
          break;
        case BreadcrumbType.Log:
          this["type"] = "log";
          break;
        case BreadcrumbType.User:
          this["type"] = "user";
          break;
        case BreadcrumbType.State:
          this["type"] = "state";
          break;
        case BreadcrumbType.Error:
          this["type"] = "error";
          break;
        case BreadcrumbType.Manual:
        default:
          this["type"] = "manual";
          break;
      }
    }
  }
}
