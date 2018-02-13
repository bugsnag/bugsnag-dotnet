using System.Collections.Generic;
using Bugsnag.Payload;
using Microsoft.AspNetCore.Http;

namespace Bugsnag.AspNet.Core
{
  public class HttpContextBreadcrumbs : Breadcrumbs
  {
    private static string _key = "Bugsnag.Breadcrumbs";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextBreadcrumbs(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    protected override List<Breadcrumb> BreadcrumbCollection
    {
      get
      {
        if (_httpContextAccessor.HttpContext != null)
        {
          var breadcrumbs = _httpContextAccessor.HttpContext.Items[_key] as List<Breadcrumb>;

          if (breadcrumbs == null)
          {
            _httpContextAccessor.HttpContext.Items[_key] = breadcrumbs = new List<Breadcrumb>();
          }

          return breadcrumbs;
        }

        return new List<Breadcrumb>();
      }
    }
  }
}
