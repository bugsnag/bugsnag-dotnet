using System.Collections.Generic;
using System.Web;
using Bugsnag.Payload;

namespace Bugsnag.AspNet
{
  public class HttpContextSessionTracking : SessionTracking
  {
    private static string _key = "Bugsnag.SessionTracking";

    public HttpContextSessionTracking(IConfiguration configuration) : base(configuration)
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
