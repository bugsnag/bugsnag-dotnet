#if NET45
using System.Web.Http.ExceptionHandling;

namespace Bugsnag.AspNet.WebApi
{
  public class ExceptionLogger : System.Web.Http.ExceptionHandling.ExceptionLogger
  {
    public override void Log(ExceptionLoggerContext context)
    {
      var exception = context.Exception;
      var request = context.Request;

      if (request.Bugsnag() != null)
      {
        request
          .Bugsnag()
          .AutoNotify(exception, request);
      }
    }
  }
}
#endif