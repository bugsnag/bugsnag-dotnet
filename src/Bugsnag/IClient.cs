using Bugsnag.Payload;

namespace Bugsnag
{
  public interface IClient
  {
    void AutoNotify(System.Exception exception);
    void AutoNotify(System.Exception exception, Request request);

    void Notify(System.Exception exception);
    void Notify(System.Exception exception, Request request);

    void Notify(System.Exception exception, Severity severity);
    void Notify(System.Exception exception, Severity severity, Request request);

    void Notify(System.Exception exception, Payload.Severity severity);
    void Notify(System.Exception exception, Payload.Severity severity, Request request);

    void Notify(Report report);

    IBreadcrumbs Breadcrumbs { get; }

    ISessionTracker SessionTracking { get; }
  }
}
