using System.Web.Http;

namespace Bugsnag.AspNet.WebApi
{
  public static class HttpConfigurationExtensions
  {
    public static void UseBugsnag(this HttpConfiguration httpConfiguration, IConfiguration configuration)
    {
      httpConfiguration.MessageHandlers.Add(new DelegatingHandler(configuration));

#if NET40
      // here we are being added to a webapi 1 application which does not
      // support the IExceptionLogger
      httpConfiguration.Filters.Add(new ExceptionFilterAttribute());
#elif NET45
      // here we are being added to a webapi 2 application, we do not add the
      // ExceptionFilterAttribute as well as that could result in double
      // sending the exception
      httpConfiguration.Services.Add(typeof(System.Web.Http.ExceptionHandling.IExceptionLogger), new ExceptionLogger());
#endif
    }
  }
}
