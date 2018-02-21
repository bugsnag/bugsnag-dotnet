using System;
using System.Web;

namespace Bugsnag.AspNet
{
  public static class ClientExtensions
  {
    public static void AutoNotify(this IClient client, Exception exception, HttpContextBase httpContext)
    {
      client.AutoNotify(exception, httpContext.ToRequest());
    }

    public static void Notify(this IClient client, Exception exception, HttpContextBase httpContext)
    {
      client.Notify(exception, httpContext.ToRequest());
    }

    public static void Notify(this IClient client, Exception exception, Severity severity, HttpContextBase httpContext)
    {
      client.Notify(exception, severity, httpContext.ToRequest());
    }

    public static void Notify(this IClient client, Exception exception, Payload.Severity severity, HttpContextBase httpContext)
    {
      client.Notify(exception, severity, httpContext.ToRequest());
    }
  }
}
