using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bugsnag.Payload
{
  public class Report : Dictionary<string, object>, ITransportablePayload
  {
    private static readonly string _payloadVersion = "4";

    private readonly KeyValuePair<string, string>[] _headers;

    private readonly ReportContext _reportContext;

    public Report(IConfiguration configuration, System.Exception exception, Severity severity, IEnumerable<Breadcrumb> breadcrumbs, Session session)
      : this(configuration, exception, severity, breadcrumbs, session, new ReportContext(exception, severity))
    {

    }

    /// <summary>
    /// Represents an error report that can be sent to the Bugsnag error notification endpoint.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="exception"></param>
    /// <param name="severity"></param>
    /// <param name="breadcrumbs"></param>
    public Report(IConfiguration configuration, System.Exception exception, Severity severity, IEnumerable<Breadcrumb> breadcrumbs, Session session, ReportContext context)
    {
      _reportContext = context;
      Deliver = true;
      Endpoint = configuration.Endpoint;
      _headers = new KeyValuePair<string, string>[] {
        new KeyValuePair<string, string>(Payload.Headers.ApiKeyHeader, configuration.ApiKey),
        new KeyValuePair<string, string>(Payload.Headers.PayloadVersionHeader, _payloadVersion),
      };

      this.AddToPayload("apiKey", configuration.ApiKey);
      this.AddToPayload("notifier", NotifierInfo.Instance);

      var app = new App(configuration);
      var device = new Device();

      this.AddToPayload("events", new[] { new Event(_payloadVersion, app, device, exception, severity, breadcrumbs, session) });
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

    /// <summary>
    /// Additional context for the report that can be used to perform
    /// additional processing on the report.
    /// </summary>
    public ReportContext Context { get { return _reportContext; } }

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

    public KeyValuePair<string, string>[] Headers { get { return _headers; } }
  }
}
