using System.Web.Http.Filters;
using System.Web.Mvc;
using Bugsnag.Clients;

namespace Web.MVCApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
#if !MONO && !NET35
            filters.Add(WebMVCClient.ErrorHandler());
#endif
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterWebApiFilters(HttpFilterCollection filters)
        {
            filters.Add(WebAPIClient.ErrorHandler());
        }
    }
}
