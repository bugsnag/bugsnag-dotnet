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

    public bool Deliver { get; set; }

    public Report(IConfiguration configuration, System.Exception exception, Severity severity)
    {
      Deliver = true;

      this["apiKey"] = configuration.ApiKey;
      this["notifier"] = NotifierInfo;
      this["events"] = new[] { new Event(configuration, exception, severity) };
    }

    public IEnumerable<Event> Events { get { return (IEnumerable<Event>)this["events"]; } }
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
