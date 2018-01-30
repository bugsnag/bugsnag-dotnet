using System;
using System.Web;

namespace Bugsnag.AspNet
{
  public class Client : Bugsnag.Client
  {
    public Client() : base(AspNet.Configuration.Settings, ThreadQueueTransport.Instance, new HttpContextStore<Payload.Breadcrumb>("Bugsnag.Breadcrumbs"))
    {

    }

    public void Notify(Exception exception, HttpContextBase httpContext)
    {
      var report = new Payload.Report(Configuration, exception, Payload.Severity.ForHandledException(), BreadcrumbStore);

      Notify(report, httpContext);
    }

    public void Notify(Exception exception, HttpContextBase httpContext, Severity severity)
    {
      var report = new Payload.Report(Configuration, exception, Payload.Severity.ForUserSpecifiedSeverity(severity), BreadcrumbStore);

      Notify(report, httpContext);

    }

    public void AutoNotify(Exception exception, HttpContextBase httpContext)
    {
      var report = new Payload.Report(Configuration, exception, Payload.Severity.ForUnhandledException(), BreadcrumbStore);

      Notify(report, httpContext);
    }

    private void Notify(Payload.Report report, HttpContextBase httpContext)
    {
      var request = new Payload.Request(new Request(httpContext));

      foreach (var @event in report.Events)
      {
        @event["request"] = request;
      }

      Notify(report);
    }
  }
}
