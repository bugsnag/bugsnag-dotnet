using Bugsnag.Payload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using System.Linq;

namespace Bugsnag.AspNet.Core
{
  public static class HttpContextExtensions
  {
    public static Request ToRequest(this HttpContext httpContext)
    {
      var ip = httpContext.Connection.RemoteIpAddress ?? httpContext.Connection.LocalIpAddress;

      return new Request
      {
        ClientIp = ip.ToString(),
        Headers = httpContext.Request.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
        HttpMethod = httpContext.Request.Method,
        Url = httpContext.Request.GetDisplayUrl(),
        Referer = httpContext.Request.Headers[HeaderNames.Referer],
      };
    }
  }
}
