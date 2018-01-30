using System;
using System.Web;
using System.Web.Mvc;

namespace Bugsnag.AspNet.Mvc
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
  public class HandleErrorAttribute : System.Web.Mvc.HandleErrorAttribute
  {
    public override void OnException(ExceptionContext filterContext)
    {
      base.OnException(filterContext);

      foreach (var key in filterContext.HttpContext.ApplicationInstance.Modules.AllKeys)
      {
        if (HttpContext.Current.ApplicationInstance.Modules[key] is HttpModule module)
        {
          module.AutoNotify(filterContext.Exception, filterContext.HttpContext);
          break;
        }
      }
    }
  }
}
