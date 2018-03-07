using Bugsnag.Payload;

namespace Bugsnag
{
  public interface IClient
  {
    void Notify(System.Exception exception, Request request = null);
    void Notify(System.Exception exception, Severity severity, Request request = null);
    void Notify(System.Exception exception, HandledState severity, Request request = null);
    void Notify(Report report);

    IBreadcrumbs Breadcrumbs { get; }

    ISessionTracker SessionTracking { get; }

    IConfiguration Configuration { get; }
  }
}
