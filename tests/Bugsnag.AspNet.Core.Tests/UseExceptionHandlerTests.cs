using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using System.Linq;
using System.Threading.Tasks;
using System;
using Xunit;

namespace Bugsnag.AspNet.Core.Tests
{
  /// <summary>
  /// Tests that Bugsnag works correctly when the UseExceptionHandler middleware
  /// is being used.
  /// </summary>
  public class UseExceptionHandlerTests : IAsyncLifetime
  {
    public string BugsnagPayload { get; set; }

    public Task DisposeAsync()
    {
      return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
      var bugsnag = new Bugsnag.Tests.TestServer();
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
      var response = await client.SendAsync(new System.Net.Http.HttpRequestMessage() { RequestUri = new Uri("/error", UriKind.Relative) });

      var bugsnags = await bugsnag.Requests(1);

      BugsnagPayload = bugsnags.First().Body;
    }

    /// <summary>
    /// Test that the context is being set correctly to the original path of the request.
    /// </summary>
    [Fact]
    public void ContextIsSetCorrectly()
    {
      Assert.Contains("\"context\":\"/error\"", BugsnagPayload);
    }

    /// <summary>
    /// Test that the url is being set correctly to the original path of the request.
    /// </summary>
    [Fact]
    public void UrlIsSetCorrectly()
    {
      Assert.Contains("\"url\":\"http://localhost/error\"", BugsnagPayload);
    }
  }
}
