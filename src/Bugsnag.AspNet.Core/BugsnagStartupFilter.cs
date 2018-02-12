using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Bugsnag.AspNet.Core
{
  /// <summary>
  /// A stratup filter to ensure that the Bugsnag middleware is
  /// executed at the start of the middleware stack.
  /// </summary>
  public class BugsnagStartupFilter : IStartupFilter
  {
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
      return builder =>
      {
        builder.UseMiddleware<Middleware>();
        next(builder);
      };
    }
  }
}
