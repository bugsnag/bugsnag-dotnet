using System;
using System.Collections.Generic;
using System.Net;

namespace Bugsnag.Payload
{
  public class Report : Dictionary<string, object>, IPayload
  {
    private static readonly string _payloadVersion = "4";

    private readonly KeyValuePair<string, string>[] _headers;

    private readonly System.Exception _originalException;

    private readonly Severity _originalSeverity;

    private readonly IConfiguration _configuration;

    /// <summary>
    /// Represents an error report that can be sent to the Bugsnag error notification endpoint.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="exception"></param>
    /// <param name="severity"></param>
    /// <param name="breadcrumbs"></param>
    public Report(IConfiguration configuration, System.Exception exception, Severity severity, Breadcrumb[] breadcrumbs, Session session, Request request)
    {
      Deliver = true;
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

      this.AddToPayload("events", new[] { new Event(_payloadVersion, app, device, exception, severity, breadcrumbs, session, request) });
    }

    /// <summary>
    /// Used to indicate to the Bugsnag client that this report should be delivered or not.
    /// This can be modified during the middleware processing based on built in and custom
    /// rules.
    /// </summary>
    public bool Deliver { get; set; }

    /// <summary>
    /// The list of Bugsnag payload events contained in this report. There is usually only a single
    /// event per payload but the Bugsnag error reporting API supports/requires this key to be an array.
    /// </summary>
    public IEnumerable<Event> Events { get { return this.Get("events") as IEnumerable<Event>; } }

    public System.Exception OriginalException => _originalException;

    public Severity OriginalSeverity => _originalSeverity;

    public IConfiguration Configuration => _configuration;

    /// <summary>
    /// Convenience method for setting the User on 'all' of the events in the
    /// payload.
    /// </summary>
    public User User
    {
      set
      {
        foreach (var @event in Events)
        {
          @event.User = value;
        }
      }
    }

    /// <summary>
    /// THe endpoint to send the error report to.
    /// </summary>
    public Uri Endpoint { get; set; }

    public IWebProxy Proxy { get; set; }

    public KeyValuePair<string, string>[] Headers { get { return _headers; } }
  }
}
