using System;
using System.Collections.Generic;
using System.Web;

namespace Bugsnag.AspNet
{
  public class Client : Bugsnag.Client
  {
    private readonly List<Middleware> _internalMiddleware;

    public Client() : this(ConfigurationSection.Configuration.Settings)
    {
    }

    public Client(IConfiguration configuration) : base(configuration, ThreadQueueTransport.Instance, new HttpContextBreadcrumbs(), new HttpContextSessionTracker(configuration))
    {
      _internalMiddleware = new List<Middleware>(DefaultInternalMiddleware) {
        AspNet.InternalMiddleware.AttachRequestInformation,
        AspNet.InternalMiddleware.SetRequestContext,
      };
    }

    protected override List<Middleware> InternalMiddleware
    {
      get
      {
        return _internalMiddleware;
      }
    }

    public void Notify(Exception exception, HttpContextBase httpContext)
    {
      Notify(exception, Payload.Severity.ForHandledException(), httpContext);
    }

    public void Notify(Exception exception, Severity severity, HttpContextBase httpContext)
    {
      Notify(exception, Payload.Severity.ForUserSpecifiedSeverity(severity), httpContext);
    }

    public void AutoNotify(Exception exception, HttpContextBase httpContext)
    {
      Notify(exception, Payload.Severity.ForUnhandledException(), httpContext);
    }

    public void Notify(Exception exception, Payload.Severity severity, HttpContextBase httpContext)
    {
      var report = new Payload.Report(Configuration, exception, severity, Breadcrumbs.Retrieve(), SessionTracking.CurrentSession);

      report.Context.HttpContext(httpContext);

      Notify(report);
    }
  }
}
