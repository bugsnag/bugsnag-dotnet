using Bugsnag;
using System.Web;
using System.Web.Mvc;

namespace BugsnagDemoMVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NotifyExceptionAttribute(MvcApplication.Bugsnag));
        }
    }
}
