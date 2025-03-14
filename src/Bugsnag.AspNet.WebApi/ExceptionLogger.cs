using System.Web.Http.ExceptionHandling;

namespace Bugsnag.AspNet.WebApi
{
  public class ExceptionLogger : System.Web.Http.ExceptionHandling.ExceptionLogger
  {
    public override void Log(ExceptionLoggerContext context)
    {
      var exception = context.Exception;
      var request = context.Request;

      var client = request.Bugsnag();

      if (client != null)
      {
        if (client.Configuration.AutoNotify)
        {
          client
            .Notify(exception, Payload.HandledState.ForUnhandledException());
        }
      }
    }
  }
}

