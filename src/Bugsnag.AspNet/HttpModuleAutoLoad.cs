namespace Bugsnag.AspNet
{
  public static class HttpModuleAutoLoad
  {
    public static void Attach()
    {
#if NET45
      System.Web.HttpApplication.RegisterModule(typeof(HttpModule));
#elif NET40
      Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(HttpModule));
#endif
    }
  }
}
