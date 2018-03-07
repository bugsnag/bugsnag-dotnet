using System;
using System.Web.Mvc;

namespace Bugsnag.AspNet.Mvc
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
  public class HandleErrorAttribute : System.Web.Mvc.HandleErrorAttribute
  {
    public override void OnException(ExceptionContext filterContext)
    {
      base.OnException(filterContext);

      if (!filterContext.ExceptionHandled)
      {
        // if the exception is unhandled then it will be caught by the http module
        return;
      }

      if (filterContext.HttpContext.Items[Client.HttpContextItemsKey] is IClient client)
      {
        if (client.Configuration.AutoNotify)
        {
          client.Notify(filterContext.Exception, Payload.HandledState.ForUnhandledException(), filterContext.HttpContext);
        }
      }
    }
  }
}
