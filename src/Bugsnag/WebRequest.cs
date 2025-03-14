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
      public AsyncCallback Callback { get; }

      public object OriginalState { get; }

      public Uri Endpoint { get; }

      public byte[] Report { get; }

      public System.Net.WebRequest Request { get; }

      public HttpWebResponse Response { get; set; }

      public WebException Exception { get; set; }

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
      public bool IsCompleted => InnerAsyncResult.IsCompleted;

      public WaitHandle AsyncWaitHandle => InnerAsyncResult.AsyncWaitHandle;

      public object AsyncState => WebRequestState.OriginalState;

      public bool CompletedSynchronously => InnerAsyncResult.CompletedSynchronously;

      public WebRequestState WebRequestState { get; }

      private IAsyncResult InnerAsyncResult { get; }

      public WebRequestAsyncResult(IAsyncResult innerAsyncResult, WebRequestState webRequestState)
      {
        InnerAsyncResult = innerAsyncResult;
        WebRequestState = webRequestState;
      }
    }

    public IAsyncResult BeginSend(Uri endpoint, IWebProxy proxy, KeyValuePair<string, string>[] headers, byte[] report, AsyncCallback callback, object state)
    {
      var request = (HttpWebRequest)System.Net.WebRequest.Create(endpoint);
      request.KeepAlive = false;
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
        if (result.WebRequestState.Response != null)
        {
          return new WebResponse(result.WebRequestState.Response.StatusCode);
        }

        if (result.WebRequestState.Exception != null)
        {
          if (result.WebRequestState.Exception.Response is HttpWebResponse response)
          {
            return new WebResponse(response.StatusCode);
          }
        }
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
        state.Exception = exception;
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
        state.Exception = exception;
        // call the original callback as we cannot continue sending the report
        state.Callback(new WebRequestAsyncResult(asynchronousResult, state));
        return;
      }

      state.Request.BeginGetResponse(new AsyncCallback(ReadCallback), state);
    }
  }

  public class WebResponse
  {
    public HttpStatusCode HttpStatusCode { get; }

    public WebResponse(HttpStatusCode httpStatusCode)
    {
      HttpStatusCode = httpStatusCode;
    }
  }
}
