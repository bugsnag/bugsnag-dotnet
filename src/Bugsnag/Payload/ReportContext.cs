using System.Collections.Generic;

namespace Bugsnag.Payload
{
  public class ReportContext : Dictionary<string, object>
  {
    public ReportContext(System.Exception exception, Severity severity)
    {
      this.AddToPayload("bugsnag.original.exception", exception);
      this.AddToPayload("bugsnag.original.severity", severity);
    }

    public System.Exception OriginalException
    {
      get { return this.Get("bugsnag.original.exception") as System.Exception; }
    }

    public Severity OriginalSeverity
    {
      get { return this.Get("bugsnag.original.severity") as Severity; }
    }
  }
}
