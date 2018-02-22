using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;

namespace Bugsnag
{
  public class WebRequest
  {
    private class TransportState
    {
      public AsyncCallback Callback { get; private set; }

      public object OriginalState { get; private set; }

      public Uri Endpoint { get; private set; }

      public byte[] Report { get; private set; }

      public System.Net.WebRequest Request { get; private set; }

      public HttpWebResponse Response { get; set; }

      public TransportState(AsyncCallback callback, object state, Uri endpoint, byte[] report, System.Net.WebRequest request)
      {
        Callback = callback;
        OriginalState = state;
        Endpoint = endpoint;
        Report = report;
        Request = request;
      }
    }

    private class TransportAsyncResult : IAsyncResult
    {
      public bool IsCompleted { get { return _innerAsyncResult.IsCompleted; } }

      public WaitHandle AsyncWaitHandle { get { return _innerAsyncResult.AsyncWaitHandle; } }

      public object AsyncState => TransportState.OriginalState;

      public bool CompletedSynchronously { get { return _innerAsyncResult.CompletedSynchronously; } }

      public TransportState TransportState => _transportState;

      private readonly IAsyncResult _innerAsyncResult;
      private readonly TransportState _transportState;

      public TransportAsyncResult(IAsyncResult innerAsyncResult, TransportState transportState)
      {
        _innerAsyncResult = innerAsyncResult;
        _transportState = transportState;
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
      var internalState = new TransportState(callback, state, endpoint, report, request);
      var asyncResult = request.BeginGetRequestStream(new AsyncCallback(WriteCallback), internalState);
      return new TransportAsyncResult(asyncResult, internalState);
    }

    public WebResponse EndSend(IAsyncResult asyncResult)
    {
      if (asyncResult is TransportAsyncResult result)
      {
        var statusCode = result.TransportState.Response.StatusCode;
        return new WebResponse(statusCode);
      }

      return null;
    }

    private void ReadCallback(IAsyncResult asynchronousResult)
    {
      var state = (TransportState)asynchronousResult.AsyncState;
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

      state.Callback(new TransportAsyncResult(asynchronousResult, state));
    }

    private void WriteCallback(IAsyncResult asynchronousResult)
    {
      var state = (TransportState)asynchronousResult.AsyncState;
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
