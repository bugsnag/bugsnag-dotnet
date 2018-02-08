using System.Collections.Generic;

namespace Bugsnag.Payload
{
  public class ReportContext : Dictionary<string, object>
  {
    public ReportContext(System.Exception exception, Severity severity)
    {
      this["bugsnag.original.exception"] = exception;
      this["bugsnag.original.severity"] = severity;
    }

    public System.Exception OriginalException
    {
      get { return this["bugsnag.original.exception"] as System.Exception; }
    }

    public Severity OriginalSeverity
    {
      get { return this["bugsnag.original.severity"] as Severity; }
    }
  }
}
