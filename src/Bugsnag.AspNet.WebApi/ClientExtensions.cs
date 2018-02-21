using System;
using System.Net.Http;

namespace Bugsnag.AspNet.WebApi
{
  public static class ClientExtensions
  {
    public static void AutoNotify(this IClient client, Exception exception, HttpRequestMessage httpRequestMessage)
    {
      client.AutoNotify(exception, httpRequestMessage.ToRequest());
    }

    public static void Notify(this IClient client, Exception exception, HttpRequestMessage httpRequestMessage)
    {
      client.Notify(exception, httpRequestMessage.ToRequest());
    }

    public static void Notify(this IClient client, Exception exception, Severity severity, HttpRequestMessage httpRequestMessage)
    {
      client.Notify(exception, severity, httpRequestMessage.ToRequest());
    }

    public static void Notify(this IClient client, Exception exception, Payload.Severity severity, HttpRequestMessage httpRequestMessage)
    {
      client.Notify(exception, severity, httpRequestMessage.ToRequest());
    }
  }
}
