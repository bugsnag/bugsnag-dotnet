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
      Bugsnag.InternalMiddleware.RemoveIgnoredExceptions,
      Bugsnag.InternalMiddleware.RemoveProjectRoots,
      Bugsnag.InternalMiddleware.DetectInProjectNamespaces,
      Bugsnag.InternalMiddleware.AttachGlobalMetadata,
      Bugsnag.InternalMiddleware.DetermineDefaultContext,
    };

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

    /// <summary>
    /// The configuration object that this client is using.
    /// </summary>
    public IConfiguration Configuration => _configuration;

    /// <summary>
    /// The breadcrumbs functionality for the client.
    /// </summary>
    public IBreadcrumbs Breadcrumbs => _breadcrumbs;

    /// <summary>
    /// The session tracking functionality for the client.
    /// </summary>
    public ISessionTracker SessionTracking => _sessionTracking;

    /// <summary>
    /// Adds middleware to be executed before an error report is sent.
    /// </summary>
    /// <param name="middleware"></param>
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

    /// <summary>
    /// Notifies Bugsnag of a handled exception with optional request information.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="request"></param>
    public void Notify(System.Exception exception, Request request = null)
    {
      Notify(exception, HandledState.ForHandledException(), request);
    }

    /// <summary>
    /// Notifies Bugsnag of a handled exception with a custom severity level
    /// and optional request information.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="severity"></param>
    /// <param name="request"></param>
    public void Notify(System.Exception exception, Severity severity, Request request = null)
    {
      Notify(exception, HandledState.ForUserSpecifiedSeverity(severity), request);
    }

    /// <summary>
    /// Notifies Bugsnag of an exception with the provided handled state and
    /// optional request information.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="severity"></param>
    /// <param name="request"></param>
    public void Notify(System.Exception exception, HandledState severity, Request request = null)
    {
      var report = new Report(_configuration, exception, severity, Breadcrumbs.Retrieve().ToArray(), SessionTracking.CurrentSession, request);

      Notify(report);
    }

    /// <summary>
    /// Notifies Bugsnag with a prebuilt error report. Running through the
    /// various defined middleware.
    /// </summary>
    /// <param name="report"></param>
    public void Notify(Report report)
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

      if (!report.Ignored)
      {
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
