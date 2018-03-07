using Microsoft.AspNetCore.Http;
using System;

namespace Bugsnag.AspNet.Core
{
  public static class ClientExtensions
  {
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
