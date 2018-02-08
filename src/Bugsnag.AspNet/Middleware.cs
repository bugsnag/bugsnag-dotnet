using System.Web;

namespace Bugsnag.AspNet
{
  public static class InternalMiddleware
  {
    public static Middleware AttachRequestInformation = (configuration, report) => {
      HttpContextBase httpContext = report.Context.HttpContext();

      if (httpContext != null)
      {
        var payloadRequest = new Request(httpContext);
        foreach (var @event in report.Events)
        {
          @event["request"] = payloadRequest;
        }
      }
    };

    public static Middleware SetRequestContext = (configuration, report) => {
      HttpContextBase httpContext = report.Context.HttpContext();

      if (httpContext != null)
      {
        foreach (var @event in report.Events)
        {
          @event.Context = httpContext.Request.RawUrl;
        }
      }
    };
  }
}
