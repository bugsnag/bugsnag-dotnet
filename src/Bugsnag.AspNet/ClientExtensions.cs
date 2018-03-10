using System;
using System.Web;

namespace Bugsnag.AspNet
{
  public static class ClientExtensions
  {
    public static void NotifyWithHttpContext(this IClient client, Exception exception)
    {
      client.Notify(exception, HttpContext.Current);
    }

    public static void NotifyWithHttpContext(this IClient client, Exception exception, Severity severity)
    {
      client.Notify(exception, severity, HttpContext.Current);
    }

    public static void NotifyWithHttpContext(this IClient client, Exception exception, Payload.HandledState handledState)
    {
      client.Notify(exception, handledState, HttpContext.Current);
    }

    public static void Notify(this IClient client, Exception exception, HttpContextBase httpContext)
    {
      client.Notify(exception, httpContext.ToRequest());
    }

    public static void Notify(this IClient client, Exception exception, Severity severity, HttpContextBase httpContext)
    {
      client.Notify(exception, severity, httpContext.ToRequest());
    }

    public static void Notify(this IClient client, Exception exception, Payload.HandledState severity, HttpContextBase httpContext)
    {
      client.Notify(exception, severity, httpContext.ToRequest());
    }

    public static void Notify(this IClient client, Exception exception, HttpContext httpContext)
    {
      client.Notify(exception, httpContext.ToRequest());
    }

    public static void Notify(this IClient client, Exception exception, Severity severity, HttpContext httpContext)
    {
      client.Notify(exception, severity, httpContext.ToRequest());
    }

    public static void Notify(this IClient client, Exception exception, Payload.HandledState severity, HttpContext httpContext)
    {
      client.Notify(exception, severity, httpContext.ToRequest());
    }
  }
}
