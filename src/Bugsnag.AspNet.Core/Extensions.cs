using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Bugsnag.AspNet.Core
{
  public static class Extensions
  {
    /// <summary>
    /// Add Bugsnag to your application. Configures the required bugsnag
    /// services and attaches the Bugsnag middleware to catch unhandled
    /// exceptions.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBugsnag(this IServiceCollection services)
    {
      return services
        .AddSingleton<IStartupFilter, BugsnagStartupFilter>()
        .AddScoped<IClient, Client>();
    }

    public static IServiceCollection AddBugsnag(this IServiceCollection services, Action<Configuration> configuration)
    {
      return services
        .AddBugsnag()
        .Configure(configuration);
    }
  }
}
