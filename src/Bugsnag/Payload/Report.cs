using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bugsnag.Payload
{
  public class Report : Dictionary<string, object>
  {
    /// <summary>
    /// A single instance of the current notifier info to attach to all error reports.
    /// </summary>
    private static Dictionary<string, string> NotifierInfo = new Dictionary<string, string> {
      { "name", ".NET Bugsnag Notifier" },
      { "version", typeof(Client).GetAssembly().GetName().Version.ToString(3) },
      { "url", "https://github.com/bugsnag/bugsnag-net" }
    };

    private readonly System.Exception _originalException;

    private readonly Severity _originalSeverity;

    /// <summary>
    /// Represents an error report that can be sent to the Bugsnag error notification endpoint.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="exception"></param>
    /// <param name="severity"></param>
    /// <param name="breadcrumbs"></param>
    public Report(IConfiguration configuration, System.Exception exception, Severity severity, IEnumerable<Breadcrumb> breadcrumbs)
    {
      Deliver = true;
      _originalException = exception;
      _originalSeverity = severity;

      this["apiKey"] = configuration.ApiKey;
      this["notifier"] = NotifierInfo;
      this["events"] = new[] { new Event(configuration, exception, severity, breadcrumbs) };
    }

    /// <summary>
    /// Used to indicate to the Bugsnag client that this report should be delivered or not.
    /// This can be modified during the middleware processing based on built in and custom
    /// rules.
    /// </summary>
    public bool Deliver { get; set; }

    /// <summary>
    /// The original exception that this report was created from.
    /// </summary>
    public System.Exception OriginalException { get { return _originalException; } }

    /// <summary>
    /// The original severity used to generate this report.
    /// </summary>
    public Severity OriginalSeverity { get { return _originalSeverity; } }

    /// <summary>
    /// The list of Bugsnag payload events contained in this report. There is usually only a single
    /// event per payload but the Bugsnag error reporting API supports/requires this key to be an array.
    /// </summary>
    public IEnumerable<Event> Events { get { return this["events"] as IEnumerable<Event>; } }
  }

  internal static class PayloadExtensions
  {
    /// <summary>
    /// Adds a key to the Bugsnag payload. This will handle not sending null or empty keys to the Bugsnag endpoint
    /// which can be a bad thing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
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

    public static void FilterPayload(this IDictionary dictionary, string[] filters)
    {
      dictionary.FilterPayload(filters, new Dictionary<object, bool>());
    }

    public static void FilterPayload(this IDictionary dictionary, string[] filters, IDictionary seen)
    {
      if (seen.Contains(dictionary))
      {
        return;
      }

      seen.Add(dictionary, true);

      foreach (var key in filters)
      {
        if (key != null && dictionary.Contains(key))
        {
          dictionary[key] = "[Filtered]"; 
        }
      }

      foreach (DictionaryEntry k in dictionary)
      {
        switch (k.Value)
        {
          case string _:
            break;
          case Uri uri:
            uri.FilterUri(filters);
            break;
          case IDictionary subDictionary:
            subDictionary.FilterPayload(filters, seen);
            break;
          case IEnumerable enumerable:
            enumerable.FilterPayload(filters, seen);
            break;
        }
      }

      seen.Remove(dictionary);
    }

    public static void FilterPayload(this IEnumerable enumerable, string[] filters, IDictionary seen)
    {
      if (seen.Contains(enumerable))
      {
        return;
      }

      seen.Add(enumerable, true);

      foreach (var item in enumerable)
      {
        switch (item)
        {
          case string _:
            break;
          case Uri uri:
            uri.FilterUri(filters);
            break;
          case IDictionary dictionary:
            dictionary.FilterPayload(filters, seen);
            break;
          case IEnumerable subEnumerable:
            subEnumerable.FilterPayload(filters, seen);
            break;
        }
      }

      seen.Remove(enumerable);
    }

    public static void FilterUri(this Uri uri, string[] filters)
    {
      // need to figure out a good way to modify this, it is basically readonly at this point
    }
  }
}
