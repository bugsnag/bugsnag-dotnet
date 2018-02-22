using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bugsnag.AspNet.WebApi
{
  public class DelegatingHandler : System.Net.Http.DelegatingHandler
  {
    private readonly IConfiguration _configuration;

    public DelegatingHandler(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var client = new Client(_configuration);
      client.SessionTracking.CreateSession();
      request.UseBugsnagClient(client);

      return base.SendAsync(request, cancellationToken);
    }
  }
}
