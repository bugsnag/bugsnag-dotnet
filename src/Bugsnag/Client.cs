using System.Collections.Generic;
using System.Diagnostics;
using Bugsnag.Payload;
using Bugsnag.SessionTracking;

namespace Bugsnag
{
  public class Client
  {
    private readonly IConfiguration _configuration;

    private readonly ITransport _transport;

    private readonly Breadcrumbs _breadcrumbs;

    private readonly SessionTracker _sessionTracking;

    private readonly List<Middleware> _middleware;

    private readonly object _middlewareLock = new object();

    private static Middleware[] InternalMiddleware = new Middleware[] {
      Bugsnag.InternalMiddleware.ReleaseStageFilter,
      Bugsnag.InternalMiddleware.RemoveIgnoredExceptions,
      Bugsnag.InternalMiddleware.RemoveFilePrefixes,
      Bugsnag.InternalMiddleware.DetectInProjectNamespaces,
      Bugsnag.InternalMiddleware.AttachGlobalMetadata,
    };

    public Client(IConfiguration configuration) : this(configuration, ThreadQueueTransport.Instance, new InMemoryBreadcrumbs(), new InMemorySessionTracker(configuration)) // wrong!
    {

    }

    public Client(IConfiguration configuration, ITransport transport, Breadcrumbs breadcrumbs, SessionTracker sessionTracking)
    {
      _configuration = configuration;
      _transport = transport;
      _breadcrumbs = breadcrumbs;
      _sessionTracking = sessionTracking;
      _middleware = new List<Middleware>();
    }

    public IConfiguration Configuration { get { return _configuration; } }

    public Breadcrumbs Breadcrumbs { get { return _breadcrumbs; } }

    public SessionTracker SessionTracking { get { return _sessionTracking; } }

    public void BeforeNotify(Middleware middleware)
    {
      lock (_middlewareLock)
      {
        _middleware.Add(middleware);
      }
    }

    public void Notify(System.Exception exception)
    {
      // TODO: get the current session and pass it in here
      var report = new Report(_configuration, exception, Payload.Severity.ForHandledException(), Breadcrumbs.Retrieve(), null);

      Notify(report);
    }

    public void Notify(System.Exception exception, Severity severity)
    {
      // TODO: get the current session and pass it in here
      var report = new Report(_configuration, exception, Payload.Severity.ForUserSpecifiedSeverity(severity), Breadcrumbs.Retrieve(), null);

      Notify(report);
    }

    public void AutoNotify(System.Exception exception)
    {
      // TODO: get the current session and pass it in here
      var report = new Report(_configuration, exception, Payload.Severity.ForUnhandledException(), Breadcrumbs.Retrieve(), null);

      Notify(report);
    }

    public void Notify(Report report)
    {
      foreach (var middleware in InternalMiddleware)
      {
        middleware(Configuration, report);
      }

      lock (_middlewareLock)
      {
        foreach (var middleware in _middleware)
        {
          try
          {
            middleware(Configuration, report);
          }
          catch (System.Exception exception)
          {
            Trace.WriteLine(exception);
            // we could add the exception calling the middleware to the report? Or to the metadata in the report
          }
        }
      }

      if (report.Deliver)
      {
        Bugsnag.InternalMiddleware.ApplyMetadataFilters(Configuration, report);

        _transport.Send(report);

        Breadcrumbs.Leave(Breadcrumb.FromReport(report));
      }
    }
  }
}
