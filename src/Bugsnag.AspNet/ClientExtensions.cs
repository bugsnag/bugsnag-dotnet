using System;
using System.Web;

namespace Bugsnag.AspNet
{
  public static class ClientExtensions
  {
    public static void AutoNotifyWithHttpContext(this IClient client, Exception exception)
    {
      client.AutoNotify(exception, HttpContext.Current);
    }

    public static void AutoNotify(this IClient client, Exception exception, HttpContext httpContext)
    {
      if (httpContext != null)
      {
        client.AutoNotify(exception, new HttpContextWrapper(httpContext));
      }
      else
      {
        client.AutoNotify(exception);
      }
    }

    public static void AutoNotify(this IClient client, Exception exception, HttpContextBase httpContext)
    {
      if (httpContext != null)
      {
        client.AutoNotify(exception, httpContext.ToRequest());
      }
      else
      {
        client.AutoNotify(exception);
      }
    }

    public static void NotifyWithHttpContext(this IClient client, Exception exception)
    {
      client.Notify(exception, HttpContext.Current);
    }

    public static void Notify(this IClient client, Exception exception, HttpContext httpContext)
    {
      if (httpContext != null)
      {
        client.Notify(exception, new HttpContextWrapper(httpContext));
      }
      else
      {
        client.Notify(exception);
      }
    }

    public static void Notify(this IClient client, Exception exception, HttpContextBase httpContext)
    {
      if (httpContext != null)
      {
        client.Notify(exception, httpContext.ToRequest());
      }
      else
      {
        client.Notify(exception);
      }
    }

    public static void NotifyWithHttpContext(this IClient client, Exception exception, Severity severity)
    {
      client.Notify(exception, severity, HttpContext.Current);
    }

    public static void Notify(this IClient client, Exception exception, Severity severity, HttpContext httpContext)
    {
      if (httpContext != null)
      {
        client.Notify(exception, severity, new HttpContextWrapper(httpContext));
      }
      else
      {
        client.Notify(exception, severity);
      }
    }

    public static void Notify(this IClient client, Exception exception, Severity severity, HttpContextBase httpContext)
    {
      if (httpContext != null)
      {
        client.Notify(exception, severity, httpContext.ToRequest());
      }
      else
      {
        client.Notify(exception, severity);
      }
    }

    public static void NotifyWithHttpContext(this IClient client, Exception exception, Payload.Severity severity)
    {
      client.Notify(exception, severity, HttpContext.Current);
    }

    public static void Notify(this IClient client, Exception exception, Payload.Severity severity, HttpContext httpContext)
    {
      if (httpContext != null)
      {
        client.Notify(exception, severity, new HttpContextWrapper(httpContext));
      }
      else
      {
        client.Notify(exception, severity);
      }
    }

    public static void Notify(this IClient client, Exception exception, Payload.Severity severity, HttpContextBase httpContext)
    {
      if (httpContext != null)
      {
        client.Notify(exception, severity, httpContext.ToRequest());
      }
      else
      {
        client.Notify(exception, severity);
      }
    }
  }

  public static class Client
  {
    public const string HttpContextItemsKey = "Bugsnag.Client";

    public static IClient Current => HttpContext.Current.Items[HttpContextItemsKey] as IClient;
  }
}
