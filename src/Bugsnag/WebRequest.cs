using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;

namespace Bugsnag
{
  public class WebRequest
  {
    private class WebRequestState
    {
      public AsyncCallback Callback { get; private set; }

      public object OriginalState { get; private set; }

      public Uri Endpoint { get; private set; }

      public byte[] Report { get; private set; }

      public System.Net.WebRequest Request { get; private set; }

      public HttpWebResponse Response { get; set; }

      public WebRequestState(AsyncCallback callback, object state, Uri endpoint, byte[] report, System.Net.WebRequest request)
      {
        Callback = callback;
        OriginalState = state;
        Endpoint = endpoint;
        Report = report;
        Request = request;
      }
    }

    private class WebRequestAsyncResult : IAsyncResult
    {
      public bool IsCompleted { get { return _innerAsyncResult.IsCompleted; } }

      public WaitHandle AsyncWaitHandle { get { return _innerAsyncResult.AsyncWaitHandle; } }

      public object AsyncState => WebRequestState.OriginalState;

      public bool CompletedSynchronously { get { return _innerAsyncResult.CompletedSynchronously; } }

      public WebRequestState WebRequestState => _webRequestState;

      private readonly IAsyncResult _innerAsyncResult;
      private readonly WebRequestState _webRequestState;

      public WebRequestAsyncResult(IAsyncResult innerAsyncResult, WebRequestState webRequestState)
      {
        _innerAsyncResult = innerAsyncResult;
        _webRequestState = webRequestState;
      }
    }

    public IAsyncResult BeginSend(Uri endpoint, IWebProxy proxy, KeyValuePair<string, string>[] headers, byte[] report, AsyncCallback callback, object state)
    {
      var request = System.Net.WebRequest.Create(endpoint);
      request.Method = "POST";
      request.ContentType = "application/json";
      if (proxy != null)
      {
        request.Proxy = proxy;
      }
      if (headers != null)
      {
        foreach (var header in headers)
        {
          request.Headers[header.Key] = header.Value;
        }
      }
      request.Headers["Bugsnag-Sent-At"] = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
      var internalState = new WebRequestState(callback, state, endpoint, report, request);
      var asyncResult = request.BeginGetRequestStream(new AsyncCallback(WriteCallback), internalState);
      return new WebRequestAsyncResult(asyncResult, internalState);
    }

    public WebResponse EndSend(IAsyncResult asyncResult)
    {
      if (asyncResult is WebRequestAsyncResult result)
      {
        var statusCode = result.WebRequestState.Response.StatusCode;
        return new WebResponse(statusCode);
      }

      return null;
    }

    private void ReadCallback(IAsyncResult asynchronousResult)
    {
      var state = (WebRequestState)asynchronousResult.AsyncState;
      try
      {
        var response = (HttpWebResponse)state.Request.EndGetResponse(asynchronousResult);
        using (var stream = response.GetResponseStream())
        {
          // we don't care about the content of the http response, only the status code
        }
        state.Response = response;
      }
      catch (WebException exception)
      {
        state.Response = exception.Response as HttpWebResponse;
      }

      state.Callback(new WebRequestAsyncResult(asynchronousResult, state));
    }

    private void WriteCallback(IAsyncResult asynchronousResult)
    {
      var state = (WebRequestState)asynchronousResult.AsyncState;
      try
      {
        using (var stream = state.Request.EndGetRequestStream(asynchronousResult))
        {
          stream.Write(state.Report, 0, state.Report.Length);
        }
      }
      catch (WebException exception)
      {
        state.Response = exception.Response as HttpWebResponse;
      }

      state.Request.BeginGetResponse(new AsyncCallback(ReadCallback), state);
    }
  }

  public class WebResponse
  {
    private readonly HttpStatusCode _httpStatusCode;

    public WebResponse(HttpStatusCode httpStatusCode)
    {
      _httpStatusCode = httpStatusCode;
    }

    public HttpStatusCode HttpStatusCode => _httpStatusCode;
  }
}
