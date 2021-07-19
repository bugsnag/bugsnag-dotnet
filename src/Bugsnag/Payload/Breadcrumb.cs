using System;
using System.Collections.Generic;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Represents an individual breadcrumb in the error report payload.
  /// </summary>
  public class Breadcrumb : Dictionary<string, object>
  {
    private const string UndefinedName = "Breadcrumb";

    /// <summary>
    /// Build a new breadcrumb from an error report. This is used to attach a previously occurring exception to the
    /// next error report.
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    public static Breadcrumb FromReport(Report report)
    {
      if (report.OriginalException != null)
      {
        var name = report.OriginalException.GetType().ToString();
        var type = BreadcrumbType.Error;
        var metadata = new Dictionary<string, string>
          {
            { "message", report.OriginalException.Message },
          };

        if (report.OriginalSeverity != null)
        {
          metadata["severity"] = report.OriginalSeverity.ToString();
        }

        return new Breadcrumb(name, type, metadata);
      }

      return null;
    }

    public Breadcrumb(string name, BreadcrumbType type) : this(name, type, null)
    {

    }

    public Breadcrumb(string name, BreadcrumbType type, IDictionary<string, string> metadata)
    {
      if (name == null) name = UndefinedName;

      this.AddToPayload("name", name);
      this.AddToPayload("timestamp", DateTime.UtcNow);
      this.AddToPayload("metaData", metadata);

      string breadcrumbType;

      switch (type)
      {
        case BreadcrumbType.Navigation:
          breadcrumbType = "navigation";
          break;
        case BreadcrumbType.Request:
          breadcrumbType = "request";
          break;
        case BreadcrumbType.Process:
          breadcrumbType = "process";
          break;
        case BreadcrumbType.Log:
          breadcrumbType = "log";
          break;
        case BreadcrumbType.User:
          breadcrumbType = "user";
          break;
        case BreadcrumbType.State:
          breadcrumbType = "state";
          break;
        case BreadcrumbType.Error:
          breadcrumbType = "error";
          break;
        case BreadcrumbType.Manual:
        default:
          breadcrumbType = "manual";
          break;
      }

      this.AddToPayload("type", breadcrumbType);
    }

    public string Name { get { return this.Get("name") as string; } }

    public IDictionary<string, string> Metadata { get { return this.Get("metaData") as IDictionary<string, string>; } }
  }
}
