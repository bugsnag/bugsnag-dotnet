using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Bugsnag.AspNet.Core
{
  /// <summary>
  /// The Bugsnag AspNetCore middleware.
  /// </summary>
  public class Middleware
  {
    public const string HttpContextItemsKey = "Bugsnag.Client";

    private readonly RequestDelegate _next;

    public Middleware(RequestDelegate requestDelegate)
    {
      _next = requestDelegate;
    }

    public async Task Invoke(HttpContext context, IClient client)
    {
      if (client.Configuration.AutoCaptureSessions)
      {
        client.SessionTracking.CreateSession();
      }

      client.BeforeNotify(report => {
        foreach (var @event in report.Events)
        {
          @event.Request = context.ToRequest();
        }
      });

      context.Items[HttpContextItemsKey] = client;

      if (client.Configuration.AutoNotify)
      {
        try
        {
          await _next(context);
        }
        catch (System.Exception exception)
        {
          client.Notify(exception, Payload.HandledState.ForUnhandledException());
          throw;
        }
      }
      else
      {
        await _next(context);
      }
    }
  }
}
