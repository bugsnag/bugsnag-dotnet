using Bugsnag.AspNet.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace aspnet45_mvc_webapi
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      config.UseBugsnag(Bugsnag.ConfigurationSection.Configuration.Settings);

      // Web API configuration and services

      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
    }
  }
}
