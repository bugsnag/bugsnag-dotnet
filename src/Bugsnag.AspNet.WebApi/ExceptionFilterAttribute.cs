using System;
using System.Web.Http.Filters;

namespace Bugsnag.AspNet.WebApi
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
  public class ExceptionFilterAttribute : System.Web.Http.Filters.ExceptionFilterAttribute
  {
    public override void OnException(HttpActionExecutedContext context)
    {
      base.OnException(context);

      var exception = context.Exception;
      var request = context.Request;

      if (request.BugsnagClient() != null)
      {
        request
          .BugsnagClient()
          .AutoNotify(exception, request);
      }
    }
  }
}
