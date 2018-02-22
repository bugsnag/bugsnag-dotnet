using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bugsnag.Payload;
using Bugsnag.SessionTracking;

namespace Bugsnag
{
  public class Client : IClient
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
      Bugsnag.InternalMiddleware.DetermineDefaultContext,
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

      UnhandledException.Instance.ConfigureClient(this);
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

    protected Middleware[] InternalMiddleware => DefaultInternalMiddleware;

    public void Notify(System.Exception exception)
    {
      Notify(exception, (Request)null);
    }

    public void Notify(System.Exception exception, Request request)
    {
      Notify(exception, Payload.Severity.ForHandledException(), request);
    }

    public void Notify(System.Exception exception, Severity severity)
    {
      Notify(exception, severity, null);
    }

    public void Notify(System.Exception exception, Severity severity, Request request)
    {
      Notify(exception, Payload.Severity.ForUserSpecifiedSeverity(severity), request);
    }

    public void AutoNotify(System.Exception exception)
    {
      AutoNotify(exception, (Request)null);
    }

    public void AutoNotify(System.Exception exception, Request request)
    {
      Notify(exception, Payload.Severity.ForUnhandledException(), request);
    }

    public void Notify(System.Exception exception, Payload.Severity severity)
    {
      Notify(exception, severity, null);
    }

    public void Notify(System.Exception exception, Payload.Severity severity, Request request)
    {
      var report = new Report(_configuration, exception, severity, Breadcrumbs.Retrieve().ToArray(), SessionTracking.CurrentSession, request);

      Notify(report);
    }

    public void Notify(Report report)
    {
      foreach (var middleware in InternalMiddleware)
      {
        try
        {
          middleware(Configuration, report);
        }
        catch (System.Exception exception)
        {
          Trace.WriteLine(exception);
        }
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
          }
        }
      }

      if (report.Deliver)
      {
        Bugsnag.InternalMiddleware.ApplyMetadataFilters(Configuration, report);

        _transport.Send(report);

        Breadcrumbs.Leave(Breadcrumb.FromReport(report));

        if (SessionTracking.CurrentSession != null)
        {
          SessionTracking.CurrentSession.AddException(report);
        }
      }
    }
  }
}
