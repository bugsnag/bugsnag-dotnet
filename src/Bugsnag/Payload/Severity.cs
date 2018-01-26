using System.Collections.Generic;

namespace Bugsnag.Payload
{
  public class Severity : Dictionary<string, object>
  {
    public static Severity ForUnhandledException()
    {
      return new Severity(false, Bugsnag.Severity.Error, SeverityReason.ForUnhandledException());
    }

    public static Severity ForHandledException()
    {
      return new Severity(true, Bugsnag.Severity.Warning, SeverityReason.ForHandledException());
    }

    public static Severity ForUserSpecifiedSeverity(Bugsnag.Severity severity)
    {
      return new Severity(true, severity, null);
    }

    public static Severity ForCallbackSpecifiedSeverity(Bugsnag.Severity severity, Severity previousSeverity)
    {
      return new Severity(previousSeverity.Handled, severity, SeverityReason.ForCallbackSpecifiedSeverity());
    }

    Severity(bool handled, Bugsnag.Severity severity, SeverityReason reason)
    {
      this["unhandled"] = !handled;
      this.AddToPayload("severityReason", reason);

      switch (severity)
      {
        case Bugsnag.Severity.Info:
          this["severity"] = "info";
          break;
        case Bugsnag.Severity.Warning:
          this["severity"] = "warning";
          break;
        default:
          this["severity"] = "error";
          break;
      }
    }

    public bool Handled
    {
      get
      {
        switch (this["unhandled"])
        {
          case bool unhandled:
            return !unhandled;
          default:
            return true;
        }
      }
      set
      {
        this["unhandled"] = !value;
      }
    }

    class SeverityReason : Dictionary<string, object>
    {
      public static SeverityReason ForUnhandledException()
      {
        return new SeverityReason("unhandledException", null);
      }

      public static SeverityReason ForHandledException()
      {
        return new SeverityReason("handledException", null);
      }

      public static SeverityReason ForUserSpecifiedSeverity()
      {
        return new SeverityReason("userSpecifiedSeverity", null);
      }

      public static SeverityReason ForCallbackSpecifiedSeverity()
      {
        return new SeverityReason("userCallbackSetSeverity", null);
      }

      SeverityReason(string type, IDictionary<string, string> attributes)
      {
        this.AddToPayload("type", type);
        this.AddToPayload("attributes", attributes);
      }
    }
  }
}
