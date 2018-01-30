using System;
using System.Collections.Generic;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Allows capturing the various keys that the Bugsnag error reporting API
  /// supports between various web frameworks.
  /// </summary>
  public interface IHttpRequest
  {
    string ClientIp { get; }

    IDictionary<string, string> Headers { get; }

    string HttpMethod { get; }

    Uri Url { get; }

    Uri Referer { get; }
  }

  /// <summary>
  /// Represents the "request" key in the error report payload.
  /// </summary>
  public class Request : Dictionary<string, object>
  {
    public Request(IHttpRequest request)
    {
      this.AddToPayload("clientIp", request.ClientIp);
      this.AddToPayload("headers", request.Headers);
      this.AddToPayload("httpMethod", request.HttpMethod);
      this.AddToPayload("url", request.Url);
      this.AddToPayload("referer", request.Referer);
    }
  }
}
