using System;
using System.Collections.Generic;

namespace Bugsnag.Payload
{
  public class Report : Dictionary<string, object>
  {
    private static Dictionary<string, string> NotifierInfo = new Dictionary<string, string> {
      { "name", ".NET Bugsnag Notifier" },
      { "version", typeof(Client).GetAssembly().GetName().Version.ToString(3) },
      { "url", "https://github.com/bugsnag/bugsnag-net" }
    };

    private readonly System.Exception _originalException;

    private readonly Bugsnag.Payload.Severity _originalSeverity;

    public bool Deliver { get; set; }

    public Report(IConfiguration configuration, System.Exception exception, Bugsnag.Payload.Severity severity, IEnumerable<Breadcrumb> breadcrumbs)
    {
      Deliver = true;
      _originalException = exception;
      _originalSeverity = severity;

      this["apiKey"] = configuration.ApiKey;
      this["notifier"] = NotifierInfo;
      this["events"] = new[] { new Event(configuration, exception, severity, breadcrumbs) };
    }

    public System.Exception OriginalException { get { return _originalException; } }

    public Bugsnag.Payload.Severity OriginalSeverity { get { return _originalSeverity; } }

    public IEnumerable<Event> Events { get { return this["events"] as IEnumerable<Event>; } }
  }

  internal static class PayloadExtensions
  {
    public static void AddToPayload<T>(this Dictionary<string, T> dictionary, string key, T value)
    {
      if (value == null)
      {
        return;
      }

      switch (value)
      {
        case System.String s:
          if (!String.IsNullOrWhiteSpace(s)) dictionary[key] = value;
          break;
        default:
          dictionary[key] = value;
          break;
      }
    }
  }
}
