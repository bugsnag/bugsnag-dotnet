using System.Collections.Generic;

namespace Bugsnag
{
  public class Report : Dictionary<string, object>
  {
    private static Dictionary<string, string> NotifierInfo = new Dictionary<string, string> {
      { "name", ".NET Bugsnag Notifier" },
      { "version", typeof(Client).GetAssembly().GetName().Version.ToString(3) },
      { "url", "https://github.com/bugsnag/bugsnag-net" }
    };

    /// <summary>
    /// The initial exception that this report was generated from
    /// </summary>
    public System.Exception Exception { get; set; }

    public Report(string apiKey, Event @event)
    {
      this["apiKey"] = apiKey;
      this["notifier"] = NotifierInfo;
      this["events"] = new[] { @event };
    }

    public string ApiKey
    {
      get
      {
        return this["apiKey"] as string;
      }
    }
  }

  public class Event : Dictionary<string, object>
  {
    public Event(Exception[] exceptions) : this(exceptions, Severity.Error, null)
    {

    }

    public Event(Exception[] exceptions, Severity severity, App app)
    {
      this["payloadVersion"] = 2;
      this["exceptions"] = exceptions;

      this.AddToPayload("app", app);

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

  public class App : Dictionary<string, object>
  {
    public App(string version, string releaseStage, string type)
    {
      this.AddToPayload("version", version);
      this.AddToPayload("releaseStage", releaseStage);
      this.AddToPayload("type", type);
    }
  }

  public class Exception : Dictionary<string, object>
  {
    public Exception(string errorClass, string message, StackTraceLine[] stackTrace)
    {
      this.AddToPayload("errorClass", errorClass);
      this.AddToPayload("message", message);
      this.AddToPayload("stacktrace", stackTrace);
    }
  }

  public class StackTraceLine : Dictionary<string, object>
  {
    public StackTraceLine(string fileName, int lineNumber, string method, bool inProject)
    {
      this.AddToPayload("file", fileName);
      this.AddToPayload("lineNumber", lineNumber);
      this.AddToPayload("method", method);
      this.AddToPayload("inProject", inProject);
    }
  }

  internal static class PayloadExtensions
  {
    public static void AddToPayload<T>(this Dictionary<string, T> dictionary, string key, T value)
    {
      if (value != null)
      {
        dictionary[key] = value;
      }
    }
  }
}
