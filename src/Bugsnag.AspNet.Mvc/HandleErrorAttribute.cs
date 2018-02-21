using System;
using System.Web.Mvc;

namespace Bugsnag.AspNet.Mvc
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
  public class HandleErrorAttribute : System.Web.Mvc.HandleErrorAttribute
  {
    private readonly IClient _client;

    public HandleErrorAttribute() : this(Bugsnag.Singleton.Client)
    {

    }

    public HandleErrorAttribute(IClient client)
    {
      if (client == null)
      {
        throw new ArgumentNullException("client");
      }

      _client = client;
    }

    public override void OnException(ExceptionContext filterContext)
    {
      base.OnException(filterContext);

      _client.AutoNotify(filterContext.Exception, filterContext.HttpContext);
    }
  }
}
