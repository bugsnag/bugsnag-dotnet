using Bugsnag.Payload;

namespace Bugsnag
{
  public interface IClient
  {
    /// <summary>
    /// Notify Bugsnag of the provided exception.
    /// </summary>
    /// <param name="exception"></param>
    void Notify(System.Exception exception);

    /// <summary>
    /// Notify Bugsnag of the provided exception and use the provided middleware
    /// for this exception.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="callback"></param>
    void Notify(System.Exception exception, Middleware callback);

    /// <summary>
    /// Notify Bugsnag of the provided exception and specified severity.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="severity"></param>
    void Notify(System.Exception exception, Severity severity);

    /// <summary>
    /// Notify Bugsnag of the provided exception, specified severity and use
    /// the provided middleware.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="severity"></param>
    /// <param name="callback"></param>
    void Notify(System.Exception exception, Severity severity, Middleware callback);

    /// <summary>
    /// Notify Bugsnag of the provided exception, using the specified handled
    /// state.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="handledState"></param>
    void Notify(System.Exception exception, HandledState handledState);

    /// <summary>
    /// Notify Bugsnag of the provided exception, using the specified handled
    /// state and use the provided middleware.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="handledState"></param>
    /// <param name="callback"></param>
    void Notify(System.Exception exception, HandledState handledState, Middleware callback);

    /// <summary>
    /// Notify Bugsnag of the prebuilt report object and use the provided
    /// middleware for this report.
    /// </summary>
    /// <param name="report"></param>
    /// <param name="callback"></param>
    void Notify(Report report, Middleware callback);

    /// <summary>
    /// The breadcrumbs for this client.
    /// </summary>
    IBreadcrumbs Breadcrumbs { get; }

    /// <summary>
    /// The session tracking functionality for this client.
    /// </summary>
    ISessionTracker SessionTracking { get; }

    /// <summary>
    /// The configuration for this client.
    /// </summary>
    IConfiguration Configuration { get; }

    /// <summary>
    /// Add a middleware function that will be called before a report is sent
    /// by the client.
    /// </summary>
    /// <param name="middleware"></param>
    void BeforeNotify(Middleware middleware);
  }
}
