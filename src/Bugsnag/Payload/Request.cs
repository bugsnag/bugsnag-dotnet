using System.Collections.Generic;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Represents the "request" key in the error report payload.
  /// </summary>
  public class Request : Dictionary<string, object>
  {
    public string ClientIp { get { return this.Get("clientIp") as string; } set { this.AddToPayload("clientIp", value); } }

    public IDictionary<string, string> Headers { get { return this.Get("headers") as IDictionary<string, string>; } set { this.AddToPayload("headers", value); } }

    public string HttpMethod { get { return this.Get("httpMethod") as string; } set { this.AddToPayload("httpMethod", value); } }

    public string Url { get { return this.Get("url") as string; } set { this.AddToPayload("url", value); } }

    public string Referer { get { return this.Get("referer") as string; } set { this.AddToPayload("referer", value); } }
  }
}
