using Bugsnag.Payload;
using System;
using System.Collections.Generic;
using Xunit;

namespace Bugsnag.Tests.Payload
{
  public class RequestTests
  {
    class TestRequest : IHttpRequest
    {
      public string ClientIp => "127.0.0.1";

      public IDictionary<string, string> Headers => new Dictionary<string, string>() { { "Header", "Wow" } };

      public string HttpMethod => "GET";

      public Uri Url => new Uri("https://bugsnag.com");

      public Uri Referer => new Uri("https://bugsnag.com");
    }

    [Fact]
    public void RequestHasTheCorrectKeys()
    {
      var testRequest = new TestRequest();

      var request = new Request(testRequest);

      Assert.Equal(testRequest.ClientIp, request["clientIp"]);
      Assert.Equal(testRequest.Headers, request["headers"]);
      Assert.Equal(testRequest.HttpMethod, request["httpMethod"]);
      Assert.Equal(testRequest.Url, request["url"]);
      Assert.Equal(testRequest.Referer, request["referer"]);
    }
  }
}
