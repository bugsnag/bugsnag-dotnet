using Bugsnag.Clients;
using System.Web.Mvc;

namespace Web.MVCApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(WebMVCClient.ErrorHandler());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
