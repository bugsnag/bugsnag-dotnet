using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Bugsnag.AspNet.Core
{
  public class Client : Bugsnag.Client, IClient
  {
    private readonly List<Bugsnag.Middleware> _internalMiddleware = null;

    public Client(IOptions<Configuration> configuration) : base(configuration.Value, ThreadQueueTransport.Instance, new InMemoryBreadcrumbs(), new SessionTracking.InMemorySessionTracker(configuration.Value))
    {
      _internalMiddleware = new List<Bugsnag.Middleware>(DefaultInternalMiddleware) {
          Core.InternalMiddleware.AttachRequestInformation,
          Core.InternalMiddleware.SetRequestContext,
        };
    }

    protected override List<Bugsnag.Middleware> InternalMiddleware
    {
      get
      {
        return _internalMiddleware;
      }
    }

    public void Notify(Exception exception, HttpContext httpContext)
    {
      Notify(exception, Payload.Severity.ForHandledException(), httpContext);
    }

    public void Notify(Exception exception, Severity severity, HttpContext httpContext)
    {
      Notify(exception, Payload.Severity.ForUserSpecifiedSeverity(severity), httpContext);
    }

    public void AutoNotify(Exception exception, HttpContext httpContext)
    {
      Notify(exception, Payload.Severity.ForUnhandledException(), httpContext);
    }

    public void Notify(Exception exception, Payload.Severity severity, HttpContext httpContext)
    {
      var report = new Payload.Report(Configuration, exception, severity, Breadcrumbs.Retrieve(), SessionTracking.CurrentSession);

      report.Context.HttpContext(httpContext);

      Notify(report);
    }
  }
}
