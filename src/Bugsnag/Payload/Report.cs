using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bugsnag.Payload
{
  public class Report : Dictionary<string, object>, ITransportablePayload
  {
    private static readonly string _payloadVersion = "4";

    private readonly System.Exception _originalException;

    private readonly Severity _originalSeverity;

    private readonly KeyValuePair<string, string>[] _headers;

    /// <summary>
    /// Represents an error report that can be sent to the Bugsnag error notification endpoint.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="exception"></param>
    /// <param name="severity"></param>
    /// <param name="breadcrumbs"></param>
    public Report(IConfiguration configuration, System.Exception exception, Severity severity, IEnumerable<Breadcrumb> breadcrumbs, Session session)
    {
      Deliver = true;
      Endpoint = configuration.Endpoint;
      _originalException = exception;
      _originalSeverity = severity;
      _headers = new KeyValuePair<string, string>[] {
        new KeyValuePair<string, string>(Payload.Headers.ApiKeyHeader, configuration.ApiKey),
        new KeyValuePair<string, string>(Payload.Headers.PayloadVersionHeader, _payloadVersion),
      };

      this["apiKey"] = configuration.ApiKey;
      this["notifier"] = NotifierInfo.Instance;

      var app = new App(configuration);
      var device = new Device();

      this["events"] = new[] { new Event(_payloadVersion, app, device, exception, severity, breadcrumbs, session) };
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

    /// <summary>
    /// THe endpoint to send the error report to.
    /// </summary>
    public Uri Endpoint { get; set; }

    public KeyValuePair<string, string>[] Headers { get { return _headers; } }
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

    public static byte[] Serialize(this IDictionary dictionary)
    {
      byte[] data = null;

      try
      {
        var payload = SimpleJson.SimpleJson.SerializeObject(dictionary);
        data = System.Text.Encoding.UTF8.GetBytes(payload);
      }
      catch (System.Exception exception)
      {
        Trace.WriteLine(exception);
      }

      return data;
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
