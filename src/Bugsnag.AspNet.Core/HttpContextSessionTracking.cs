using Bugsnag.Payload;
using Bugsnag.SessionTracking;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Bugsnag.AspNet.Core
{
  public class HttpContextSessionTracking : SessionTracker
  {
    private static string _key = "Bugsnag.SessionTracking";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextSessionTracking(IHttpContextAccessor httpContextAccessor, IOptions<Configuration> configuration) : base(configuration.Value)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public override Session CurrentSession
    {
      get
      {
        if (_httpContextAccessor.HttpContext != null)
        {
          switch (_httpContextAccessor.HttpContext.Items[_key])
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
        if (_httpContextAccessor.HttpContext != null)
        {
          _httpContextAccessor.HttpContext.Items[_key] = value;
        }
      }
    }
  }
}
