using System.Collections.Generic;
using System.Web;
using Bugsnag.Payload;

namespace Bugsnag.AspNet
{
  public class HttpContextBreadcrumbs : Breadcrumbs
  {
    private static string _key = "Bugsnag.Breadcrumbs";

    protected override List<Breadcrumb> BreadcrumbCollection
    {
      get
      {
        if (HttpContext.Current != null)
        {
          var breadcrumbs = HttpContext.Current.Items[_key] as List<Breadcrumb>;

          if (breadcrumbs == null)
          {
            HttpContext.Current.Items[_key] = breadcrumbs = new List<Breadcrumb>();
          }

          return breadcrumbs;
        }

        return new List<Breadcrumb>();
      }
    }
  }
}
