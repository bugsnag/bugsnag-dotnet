using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using System.Linq;

namespace Bugsnag.AspNet.Core
{
  /// <summary>
  /// Translates between the AspNetCore HttpContext and what Bugsnag needs to
  /// send as part of the error reports it sends.
  /// </summary>
  class Request : Payload.IHttpRequest
  {
    private readonly string _clientIp;
    private readonly IDictionary<string, string> _headers;
    private readonly string _httpMethod;
    private readonly string _url;
    private readonly string _referer;

    public Request(HttpContext context)
    {
      var ip = context.Connection.RemoteIpAddress ?? context.Connection.LocalIpAddress;

      _clientIp = ip.ToString();
      _httpMethod = context.Request.Method;
      _url = context.Request.GetDisplayUrl();
      _referer = context.Request.Headers[HeaderNames.Referer];
      _headers = context.Request.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value));
    }

    public string ClientIp => _clientIp;

    public IDictionary<string, string> Headers => _headers;

    public string HttpMethod => _httpMethod;

    public string Url => _url;

    public string Referer => _referer;
  }
}
