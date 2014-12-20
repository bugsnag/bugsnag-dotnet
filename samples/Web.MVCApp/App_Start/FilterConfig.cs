using System.Web.Http.Filters;
using System.Web.Mvc;
using Bugsnag.Clients;

namespace Web.MVCApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(WebMVCClient.ErrorHandler());
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterWebApiFilters(HttpFilterCollection filters)
        {
            filters.Add(WebAPIClient.ErrorHandler());
        }
    }
}
