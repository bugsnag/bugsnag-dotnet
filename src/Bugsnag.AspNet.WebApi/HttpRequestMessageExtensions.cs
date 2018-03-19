using Bugsnag.Payload;
using System.Linq;
using System.Net.Http;

namespace Bugsnag.AspNet.WebApi
{
  public static class HttpRequestMessageExtensions
  {
    public const string HttpContextItemsKey = "Bugsnag.Client";

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
      request.Properties[HttpContextItemsKey] = client;
    }

    public static IClient Bugsnag(this HttpRequestMessage request)
    {
      if (request.Properties.ContainsKey(HttpContextItemsKey))
      {
        return request.Properties[HttpContextItemsKey] as IClient;
      }

      return null;
    }
  }
}
