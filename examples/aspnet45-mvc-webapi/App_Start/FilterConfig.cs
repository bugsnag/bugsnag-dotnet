using System.Web;
using System.Web.Mvc;

namespace aspnet45_mvc_webapi
{
  public class FilterConfig
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new Bugsnag.AspNet.Mvc.HandleErrorAttribute());
    }
  }
}
