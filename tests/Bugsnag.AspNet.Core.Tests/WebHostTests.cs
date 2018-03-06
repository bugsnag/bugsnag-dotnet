using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bugsnag.AspNet.Core.Tests
{
  public class WebHostTests
  {
    [Fact]
    public async void TestWithNoExceptions()
    {
      var builder = new WebHostBuilder()
        .ConfigureServices(services => services.AddBugsnag(config => { config.ApiKey = "123456"; }))
        .Configure(app => {
          app.Run(async context => {
            await context.Response.WriteAsync("OK");
          });
        });

      var server = new TestServer(builder);
      var client = server.CreateClient();
      var response = await client.SendAsync(new System.Net.Http.HttpRequestMessage());

      Assert.NotNull(response);
    }

    [Fact]
    public async void TestWithDeveloperExceptionPage()
    {
      var bugsnag = new Bugsnag.Tests.TestServer(1);
      bugsnag.Start();

      var builder = new WebHostBuilder()
        .ConfigureServices(services => services.AddBugsnag(config => { config.ApiKey = "123456"; config.Endpoint = bugsnag.Endpoint; }))
        .Configure(app => {
          app.UseDeveloperExceptionPage();
          app.Run(context => {
            throw new System.Exception("a serious error");
          });
        });

      var server = new TestServer(builder);
      var client = server.CreateClient();
      var response = await client.SendAsync(new System.Net.Http.HttpRequestMessage());

      var bugsnags = await bugsnag.Requests();

      Assert.NotNull(response);
    }

    [Fact]
    public async void TestWithExceptionHandler()
    {
      var bugsnag = new Bugsnag.Tests.TestServer(1);
      bugsnag.Start();

      var builder = new WebHostBuilder()
        .ConfigureServices(services => services.AddBugsnag(config => { config.ApiKey = "123456"; config.Endpoint = bugsnag.Endpoint; }))
        .Configure(app => {
          app.UseExceptionHandler("/home");
          app.Map("/error", error => {
            error.Run(handler => {
              throw new System.Exception("a serious error");
            });
          });
          app.Map("/home", home => {
            home.Run(async handler => {
              await handler.Response.WriteAsync("OK");
            });
          });
        });

      var server = new TestServer(builder);
      var client = server.CreateClient();
      var response = await client.SendAsync(new System.Net.Http.HttpRequestMessage() { RequestUri = new System.Uri("/error", System.UriKind.Relative) });

      var bugsnags = await bugsnag.Requests();

      Assert.NotNull(response);
    }
  }
}
