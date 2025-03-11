#if NET462
using System.Web.Mvc;

namespace Bugsnag.AspNet.Mvc
{
  public static class AutoLoad
  {
    public static void Attach()
    {
      GlobalFilters.Filters.Add(new HandleErrorAttribute());
    }
  }
}
#endif
