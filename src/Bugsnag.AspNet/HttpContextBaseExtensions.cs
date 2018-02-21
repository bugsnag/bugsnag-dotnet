using Bugsnag.Payload;
using System.Linq;
using System.Web;

namespace Bugsnag.AspNet
{
  public static class HttpContextBaseExtensions
  {
    public static Request ToRequest(this HttpContextBase httpContext)
    {
      if (httpContext == null) return null;

      var request = httpContext.Request;

      if (request == null) return null;

      var clientIp = request.UserHostAddress;
      var headers = request.Headers.AllKeys
        .ToDictionary(k => k, k => request.Headers[k]);
      var httpMethod = request.HttpMethod;
      var url = request.Url?.ToString();
      var referer = request.UrlReferrer?.ToString();

      return new Request
      {
        ClientIp = clientIp,
        Headers = headers,
        HttpMethod = httpMethod,
        Url = url,
        Referer = referer,
      };
    }
  }
}
