using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

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
      services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      // configure the delivery once here to avoid creating a new HttpClient
      // for every request when a proxy is set in the configuration.
      services.AddSingleton<IStartupFilter>(provider => {
         var configuration = provider.GetService<IOptions<Configuration>>();
         DefaultDelivery.Instance.Configure(configuration.Value);
         return new BugsnagStartupFilter();
       });

      return services
        .AddScoped<IClient, Client>(context => {
          var configuration = context.GetService<IOptions<Configuration>>();
          var client = new Client(configuration.Value, DefaultDelivery.Instance);
          return client;
        });
    }

    public static IServiceCollection AddBugsnag(this IServiceCollection services, Action<Configuration> configuration)
    {
      return services
        .AddBugsnag()
        .Configure(configuration);
    }
  }
}
