namespace Bugsnag.AspNet
{
  public static class HttpModuleAutoLoad
  {
    public static void Attach()
    {
      System.Web.HttpApplication.RegisterModule(typeof(HttpModule));
    }
  }
}
