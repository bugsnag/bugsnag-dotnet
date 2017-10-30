#if NET45
using System.Web;

namespace Bugsnag.AspNet
{
  public static class HttpModuleAutoLoad
  {
    public static void Attach()
    {
      HttpApplication.RegisterModule(typeof(HttpModule));
    }
  }
}
#endif
