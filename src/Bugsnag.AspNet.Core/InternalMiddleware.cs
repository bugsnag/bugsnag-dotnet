using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Bugsnag.AspNet.Core
{
  /// <summary>
  /// The stack of additional middleware the the AspNetCore Bugsnag client uses.
  /// </summary>
  public static class InternalMiddleware
  {
    /// <summary>
    /// Ensure that any provided request information is attached to the error
    /// report.
    /// </summary>
    public static Bugsnag.Middleware AttachRequestInformation = (configuration, report) => {
      HttpContext httpContext = report.Context.HttpContext();

      if (httpContext != null)
      {
        var payloadRequest = new Payload.Request(new Request(httpContext));
        foreach (var @event in report.Events)
        {
          @event["request"] = payloadRequest;
        }
      }
    };

    /// <summary>
    /// Sets the context of the error report to the current url when an error
    /// occurs during a web request.
    /// </summary>
    public static Bugsnag.Middleware SetRequestContext = (configuration, report) => {
      HttpContext httpContext = report.Context.HttpContext();

      if (httpContext != null)
      {
        foreach (var @event in report.Events)
        {
          @event.Context = httpContext.Request.GetDisplayUrl();
        }
      }
    };
  }
}
