using System.Collections.Generic;

namespace Bugsnag
{
  public class Event : Dictionary<string, object>
  {
    public Event(IConfiguration configuration, System.Exception exception, Severity severity)
    {
      this["payloadVersion"] = 2;
      this["exceptions"] = new Exceptions(exception);
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
  }
}
