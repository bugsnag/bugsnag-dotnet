using Bugsnag.Web;
using System.Web.Mvc;

namespace Web.MVCApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new NotifyExceptionAttribute("9134c4469d16f30f025a1e98f45b3ddb"));
            filters.Add(new HandleErrorAttribute());

        }
    }
}
