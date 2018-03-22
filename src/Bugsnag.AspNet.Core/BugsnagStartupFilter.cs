using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DiagnosticAdapter;

namespace Bugsnag.AspNet.Core
{
  /// <summary>
  /// A startup filter to ensure that the Bugsnag middleware is
  /// executed at the start of the middleware stack.
  /// </summary>
  public class BugsnagStartupFilter : IStartupFilter
  {
    static BugsnagStartupFilter()
    {
      // populate the env variable that the client expects with the netcore provided value
      Environment.SetEnvironmentVariable("BUGSNAG_RELEASE_STAGE", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
      return builder =>
      {
        builder.ApplicationServices.GetService<DiagnosticListener>()?.SubscribeWithAdapter(new DiagnosticSubscriber());
        builder.UseMiddleware<Middleware>();
        next(builder);
      };
    }

    private class DiagnosticSubscriber
    {
      /// <summary>
      /// Handles exceptions that the Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware
      /// swallows.
      /// </summary>
      /// <param name="exception"></param>
      /// <param name="httpContext"></param>
      [DiagnosticName("Microsoft.AspNetCore.Diagnostics.HandledException")]
      public virtual void OnHandledException(Exception exception, HttpContext httpContext)
      {
        LogException(exception, httpContext);
      }

      /// <summary>
      /// Handles exceptions that the Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware
      /// swallows.
      /// </summary>
      /// <param name="exception"></param>
      /// <param name="httpContext"></param>
      [DiagnosticName("Microsoft.AspNetCore.Diagnostics.UnhandledException")]
      public virtual void OnUnhandledException(Exception exception, HttpContext httpContext)
      {
        LogException(exception, httpContext);
      }

      private void LogException(Exception exception, HttpContext httpContext)
      {
        httpContext.Items.TryGetValue(Middleware.HttpContextItemsKey, out object clientObject);

        if (clientObject is IClient client)
        {
          if (client.Configuration.AutoNotify)
          {
            client.Notify(exception, Payload.HandledState.ForUnhandledException());
          }
        }
      }
    }
  }
}
