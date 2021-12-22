using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Bugsnag.Payload
{
  public class Report : Dictionary<string, object>, IPayload
  {
    private static readonly string _payloadVersion = "4";

    /// <summary>
    /// The maximum size of the serialized payload which can be sent
    /// to Bugsnag. The report will be trimmed if it exceeds this size.
    /// </summary>
    private static readonly int MaximumSize = 1024 * 1024;

    private readonly KeyValuePair<string, string>[] _headers;

    private readonly System.Exception _originalException;

    private readonly HandledState _originalSeverity;

    private readonly IConfiguration _configuration;

    private readonly Event _event;

    private bool _ignored;

    /// <summary>
    /// Represents an error report that can be sent to the Bugsnag error notification endpoint.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="exception"></param>
    /// <param name="severity"></param>
    /// <param name="breadcrumbs"></param>
    /// <param name="session"></param>
    public Report(IConfiguration configuration, System.Exception exception, HandledState severity, IEnumerable<Breadcrumb> breadcrumbs, Session session)
    {
      _ignored = false;
      Endpoint = configuration.Endpoint;
      Proxy = configuration.Proxy;
      _headers = new KeyValuePair<string, string>[] {
        new KeyValuePair<string, string>(Payload.Headers.ApiKeyHeader, configuration.ApiKey),
        new KeyValuePair<string, string>(Payload.Headers.PayloadVersionHeader, _payloadVersion),
      };

      _configuration = configuration;
      _originalException = exception;
      _originalSeverity = severity;

      this.AddToPayload("apiKey", configuration.ApiKey);
      this.AddToPayload("notifier", NotifierInfo.Instance);

      var app = new App(configuration);
      var device = new Device();

      _event = new Event(_payloadVersion, app, device, exception, severity, breadcrumbs, session);

      this.AddToPayload("events", new[] { _event });
    }

    /// <summary>
    /// Used to indicate to the Bugsnag client that this report should not be delivered.
    /// </summary>
    public void Ignore()
    {
      _ignored = true;
    }

    public bool Ignored => _ignored;

    /// <summary>
    /// The list of Bugsnag payload events contained in this report. There is usually only a single
    /// event per payload but the Bugsnag error reporting API supports/requires this key to be an array.
    /// </summary>
    public Event Event => _event;

    public System.Exception OriginalException => _originalException;

    public HandledState OriginalSeverity => _originalSeverity;

    public IConfiguration Configuration => _configuration;

    /// <summary>
    /// THe endpoint to send the error report to.
    /// </summary>
    public Uri Endpoint { get; set; }

    public IWebProxy Proxy { get; set; }

    public KeyValuePair<string, string>[] Headers { get { return _headers; } }

    public byte[] Serialize()
    {
      byte[] data = null;

      try
      {
        data = Serializer.SerializeObjectToByteArray(this, _configuration.MetadataFilters);

        if (data.Length > MaximumSize && Event.TrimExtraData())
        {
          data = Serializer.SerializeObjectToByteArray(this, _configuration.MetadataFilters);
        }
      }
      catch (System.Exception exception)
      {
        Trace.WriteLine(exception);
      }

      return data;
    }
  }
}
