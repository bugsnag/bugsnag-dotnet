using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Bugsnag.AspNet.Core
{
  /// <summary>
  /// The Bugsnag AspNetCore middleware.
  /// </summary>
  public class Middleware
  {
    private readonly RequestDelegate _next;

    public Middleware(RequestDelegate requestDelegate)
    {
      _next = requestDelegate;
    }

    public async Task Invoke(HttpContext context, IClient client, Microsoft.Extensions.Configuration.IConfiguration root)
    {
      try
      {
        await _next(context);
      }
      catch (System.Exception exception)
      {
        client.AutoNotify(exception, context);
        throw;
      }

      // check that ExceptionHandlerMiddleware has not swallowed any exceptions
      var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

      if (exceptionFeature != null)
      {
        client.AutoNotify(exceptionFeature.Error, context);
      }
    }
  }
}
