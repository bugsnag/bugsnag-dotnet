using System;
using System.Web;

namespace Bugsnag.AspNet
{
  public class Client : Bugsnag.Client
  {
    public Client() : base(AspNet.Configuration.Settings, ThreadQueueTransport.Instance, new HttpContextBreadcrumbs(), new HttpContextSessionTracker(AspNet.Configuration.Settings))
    {

    }

    public void Notify(Exception exception, HttpContextBase httpContext)
    {
      var report = new Payload.Report(Configuration, exception, Payload.Severity.ForHandledException(), Breadcrumbs.Retrieve(), SessionTracking.CurrentSession);

      Notify(report, httpContext);
    }

    public void Notify(Exception exception, HttpContextBase httpContext, Severity severity)
    {
      var report = new Payload.Report(Configuration, exception, Payload.Severity.ForUserSpecifiedSeverity(severity), Breadcrumbs.Retrieve(), SessionTracking.CurrentSession);

      Notify(report, httpContext);

    }

    public void AutoNotify(Exception exception, HttpContextBase httpContext)
    {
      var report = new Payload.Report(Configuration, exception, Payload.Severity.ForUnhandledException(), Breadcrumbs.Retrieve(), SessionTracking.CurrentSession);

      Notify(report, httpContext);
    }

    private void Notify(Payload.Report report, HttpContextBase httpContext)
    {
      var request = new Request(httpContext);
      var payloadRequest = new Payload.Request(request);

      foreach (var @event in report.Events)
      {
        @event.Context = httpContext.Request.RawUrl;
        @event["request"] = request;
      }

      Notify(report);
    }
  }
}
