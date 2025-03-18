using Bugsnag.Tests;
using System;
using System.Web;
using Xunit;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Linq;

namespace Bugsnag.AspNet.Tests
{
  public class ClientTests : IAsyncLifetime
  {
    private string _request;

    public Task DisposeAsync()
    {
      return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
      var server = new TestServer();

      server.Start();

      var client = new Bugsnag.Client(new Bugsnag.Configuration("123456") { Endpoint = server.Endpoint });

      try
      {
        throw new NotImplementedException();
      }
      catch (Exception e)
      {
        var context = new BugsnagHttpContext();
        client.Notify(e, report => {
          report.Event.Request = context.ToRequest();
        });
      }

      var requests = await server.Requests(1);

      _request = requests.Single().Body;
    }

    [Fact]
    public void ClientIpIsSet()
    {
      Assert.Contains("\"clientIp\":\"127.0.0.1\"", _request);
    }

    [Fact]
    public void HeadersAreSet()
    {
      Assert.Contains("\"headers\":{}", _request);
    }

    [Fact]
    public void HttpMethodIsSet()
    {
      Assert.Contains("\"httpMethod\":\"GET\"", _request);
    }

    [Fact]
    public void UrlIsSet()
    {
      Assert.Contains("\"url\":\"https://www.bugsnag.com/\"", _request);
    }

    [Fact]
    public void UrlRefererIsSet()
    {
      Assert.Contains("\"referer\":\"https://google.com/\"", _request);
    }

    [Fact]
    public void ContextIsSet()
    {
      Assert.Contains("\"context\":\"/\"", _request);
    }

    class BugsnagHttpContext : HttpContextBase
    {
      public override HttpRequestBase Request { get; } = new BugsnagHttpRequest();
    }

    class BugsnagHttpRequest : HttpRequestBase
    {
      public override string UserHostAddress => "127.0.0.1";

      public override string HttpMethod => "GET";

      public override Uri Url { get; } = new Uri("https://www.bugsnag.com");

      public override Uri UrlReferrer { get; } = new Uri("https://google.com");

      public override NameValueCollection Headers { get; } = new NameValueCollection();

      public override string RawUrl => "https://www.bugsnag.com";
    }
  }
}
