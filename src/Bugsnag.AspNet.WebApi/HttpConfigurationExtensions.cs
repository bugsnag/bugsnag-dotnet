using System.Web.Http;

namespace Bugsnag.AspNet.WebApi
{
  public static class HttpConfigurationExtensions
  {
    public static void UseBugsnag(this HttpConfiguration httpConfiguration, IConfiguration configuration)
    {
      httpConfiguration.MessageHandlers.Add(new DelegatingHandler(configuration));
      httpConfiguration.Services.Add(typeof(System.Web.Http.ExceptionHandling.IExceptionLogger), new ExceptionLogger());
    }
  }
}
