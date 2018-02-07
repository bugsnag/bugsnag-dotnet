using System;
using System.Collections.Generic;
using Bugsnag.Payload;

namespace Bugsnag
{
  public class ThreadStaticBreadcrumbs : Breadcrumbs
  {
    [ThreadStatic]
    private static List<Breadcrumb> _breadcrumbs;

    protected override List<Breadcrumb> BreadcrumbCollection
    {
      get
      {
        if (_breadcrumbs == null)
        {
          _breadcrumbs = new List<Breadcrumb>();
        }

        return _breadcrumbs;
      }
    }
  }
}
