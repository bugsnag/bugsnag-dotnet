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

    private readonly IBreadcrumbs _breadcrumbs;

    private readonly ISessionTracker _sessionTracking;

    private readonly List<Middleware> _middleware;

    private readonly object _middlewareLock = new object();

    protected static Middleware[] DefaultInternalMiddleware = new Middleware[] {
      Bugsnag.InternalMiddleware.ReleaseStageFilter,
      Bugsnag.InternalMiddleware.RemoveIgnoredExceptions,
      Bugsnag.InternalMiddleware.RemoveProjectRoots,
      Bugsnag.InternalMiddleware.DetectInProjectNamespaces,
      Bugsnag.InternalMiddleware.AttachGlobalMetadata,
    };

    public Client(IConfiguration configuration) : this(configuration, ThreadQueueTransport.Instance, new InMemoryBreadcrumbs(), new InMemorySessionTracker(configuration)) // wrong!
    {

    }

    public Client(IConfiguration configuration, ITransport transport, IBreadcrumbs breadcrumbs, ISessionTracker sessionTracking)
    {
      _configuration = configuration;
      _transport = transport;
      _breadcrumbs = breadcrumbs;
      _sessionTracking = sessionTracking;
      _middleware = new List<Middleware>();
    }

    public IConfiguration Configuration => _configuration;

    public IBreadcrumbs Breadcrumbs => _breadcrumbs;

    public ISessionTracker SessionTracking => _sessionTracking;

    public void BeforeNotify(Middleware middleware)
    {
      lock (_middlewareLock)
      {
        _middleware.Add(middleware);
      }
    }

    protected virtual List<Middleware> InternalMiddleware { get; } = new List<Middleware>(DefaultInternalMiddleware);

    public void Notify(System.Exception exception)
    {
      Notify(exception, Payload.Severity.ForHandledException());
    }

    public void Notify(System.Exception exception, Severity severity)
    {
      Notify(exception, Payload.Severity.ForUserSpecifiedSeverity(severity));
    }

    public void AutoNotify(System.Exception exception)
    {
      Notify(exception, Payload.Severity.ForUnhandledException());
    }

    public void Notify(System.Exception exception, Payload.Severity severity)
    {
      var report = new Report(_configuration, exception, severity, Breadcrumbs.Retrieve(), SessionTracking.CurrentSession);

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
