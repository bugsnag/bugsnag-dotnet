using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bugsnag.Payload;

namespace Bugsnag
{
  /// <summary>
  /// The Bugsnag client is used to process and send error reports to Bugsnag.
  /// </summary>
  public class Client : IClient
  {
    private readonly IConfiguration _configuration;

    private readonly IDelivery _delivery;

    private readonly IBreadcrumbs _breadcrumbs;

    private readonly ISessionTracker _sessionTracking;

    private readonly List<Middleware> _middleware;

    private readonly object _middlewareLock = new object();

    /// <summary>
    /// The default middleware that the Bugsnag client will always run before
    /// running any user specified middleware and sending the error report.
    /// </summary>
    protected static Middleware[] DefaultInternalMiddleware = new Middleware[] {
      Bugsnag.InternalMiddleware.ReleaseStageFilter,
      Bugsnag.InternalMiddleware.CheckIgnoreClasses,
      Bugsnag.InternalMiddleware.RemoveProjectRoots,
      Bugsnag.InternalMiddleware.DetectInProjectNamespaces,
      Bugsnag.InternalMiddleware.AttachGlobalMetadata,
    };

    /// <summary>
    /// Constructs a client with the given apiKey using the default configuration values.
    /// </summary>
    /// <param name="apiKey"></param>
    public Client(string apiKey) : this(new Configuration(apiKey))
    {

    }

    /// <summary>
    /// Constructs a client with the default storage and delivery classes.
    /// </summary>
    /// <param name="configuration"></param>
    public Client(IConfiguration configuration) : this(configuration, ThreadQueueDelivery.Instance, new Breadcrumbs(configuration), new SessionTracker(configuration))
    {

    }

    /// <summary>
    /// Constructs a client with the specified storage and delivery classes.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="delivery"></param>
    /// <param name="breadcrumbs"></param>
    /// <param name="sessionTracking"></param>
    public Client(IConfiguration configuration, IDelivery delivery, IBreadcrumbs breadcrumbs, ISessionTracker sessionTracking)
    {
      _configuration = configuration;
      _delivery = delivery;
      _breadcrumbs = breadcrumbs;
      _sessionTracking = sessionTracking;
      _middleware = new List<Middleware>();

      UnhandledException.Instance.ConfigureClient(this, configuration);
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

    /// <summary>
    /// The default middleware that the Bugsnag client will always run before
    /// running any user specified middleware and sending the error report.
    /// </summary>
    protected Middleware[] InternalMiddleware => DefaultInternalMiddleware;

    public void Notify(System.Exception exception)
    {
      Notify(exception, (Middleware)null);
    }

    public void Notify(System.Exception exception, Middleware callback)
    {
      Notify(exception, HandledState.ForHandledException(), callback);
    }

    public void Notify(System.Exception exception, Severity severity)
    {
      Notify(exception, severity, null);
    }

    public void Notify(System.Exception exception, Severity severity, Middleware callback)
    {
      Notify(exception, HandledState.ForUserSpecifiedSeverity(severity), callback);
    }

    public void Notify(System.Exception exception, HandledState handledState)
    {
      Notify(exception, handledState, null);
    }

    public void Notify(System.Exception exception, HandledState handledState, Middleware callback)
    {
      var report = new Report(_configuration, exception, handledState, Breadcrumbs.Retrieve().ToArray(), SessionTracking.CurrentSession);

      Notify(report, callback);
    }

    public void Notify(Report report, Middleware callback)
    {
      foreach (var middleware in InternalMiddleware)
      {
        try
        {
          middleware(report);
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
            middleware(report);
          }
          catch (System.Exception exception)
          {
            Trace.WriteLine(exception);
          }
        }
      }

      try
      {
        callback?.Invoke(report);
      }
      catch (System.Exception exception)
      {
        Trace.WriteLine(exception);
      }

      if (!report.Ignored)
      {
        Bugsnag.InternalMiddleware.DetermineDefaultContext(report);
        Bugsnag.InternalMiddleware.ApplyMetadataFilters(report);

        _delivery.Send(report);

        Breadcrumbs.Leave(Breadcrumb.FromReport(report));

        if (SessionTracking.CurrentSession != null)
        {
          SessionTracking.CurrentSession.AddException(report);
        }
      }
    }
  }
}
