using System.Web;
using Bugsnag.Payload;
using Bugsnag.SessionTracking;

namespace Bugsnag.AspNet
{
  public class HttpContextSessionTracker : SessionTracker
  {
    private static string _key = "Bugsnag.SessionTracking";

    public HttpContextSessionTracker(IConfiguration configuration) : base(configuration)
    {

    }

    public override Session CurrentSession
    {
      get
      {
        if (HttpContext.Current != null)
        {
          switch (HttpContext.Current.Items[_key])
          {
            case Session s:
              return s;
            default:
              return null;
          }
        }

        return null;
      }
      protected set
      {
        if (HttpContext.Current != null)
        {
          HttpContext.Current.Items[_key] = value;
        }
      }
    }
  }
}
