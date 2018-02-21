using Bugsnag.Payload;
using System.Linq;
using System.Net.Http;

namespace Bugsnag.AspNet.WebApi
{
  public static class HttpRequestMessageExtensions
  {
    public static Request ToRequest(this HttpRequestMessage request)
    {
      return new Request
      {
        Headers = request.Headers.ToDictionary(k => k.Key, v => string.Join(", ", v.Value)),
        HttpMethod = request.Method?.Method,
        Url = request.RequestUri?.ToString(),
        Referer = request.Headers.Referrer?.ToString(),
      };
    }

    public static void UseBugsnagClient(this HttpRequestMessage request, IClient client)
    {
      request.Properties["Bugsnag.Client"] = client;
    }

    public static IClient BugsnagClient(this HttpRequestMessage request)
    {
      return request.Properties["Bugsnag.Client"] as IClient;
    }
  }
}
