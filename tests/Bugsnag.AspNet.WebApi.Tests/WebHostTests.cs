using Bugsnag.Tests;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Xunit;

namespace Bugsnag.AspNet.WebApi.Tests
{
  public class WebHostTests
  {
    public class TestController : ApiController
    {
      [HttpGet]
      public IHttpActionResult Test()
      {
        Request.Bugsnag().Breadcrumbs.Leave("Bugsnag is great!");
        throw new NotImplementedException("Because it lets me know about exceptions");
      }
    }

    [Fact]
    public async Task Test()
    {
      var bugsnagServer = new TestServer();

      bugsnagServer.Start();

      var configuration = new HttpConfiguration();
      configuration.Routes.MapHttpRoute("Default", "api/{controller}");
      configuration.UseBugsnag(new Configuration("wow") { Endpoint = bugsnagServer.Endpoint });
      var webApiServer = new HttpServer(configuration);

      var client = new HttpClient(webApiServer);

      var request = new HttpRequestMessage() { RequestUri = new Uri("http://www.bugsnag.com/api/test") };

      var response = await client.SendAsync(request);

      var responses = await bugsnagServer.Requests(1);

      Assert.Single(responses);
      Assert.Contains("Bugsnag is great!", responses.Single());
    }
  }
}
