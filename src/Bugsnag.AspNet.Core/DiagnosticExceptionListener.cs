using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Reflection;

namespace Bugsnag.AspNet.Core
{
  internal class DiagnosticExceptionListener : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object>>
  {
    private static object _instanceLock = new object();
    private static DiagnosticExceptionListener _instance;

    private readonly object _clientLock = new object();
    private IClient _currentClient;

    private const string ExceptionHandlerMiddlewareKey = "Microsoft.AspNetCore.Diagnostics.HandledException";
    private const string DeveloperExceptionPageMiddleware = "Microsoft.AspNetCore.Diagnostics.UnhandledException";

    private DiagnosticExceptionListener()
    {
      DiagnosticListener.AllListeners.Subscribe(this);
    }

    public static DiagnosticExceptionListener Instance
    {
      get
      {
        lock (_instanceLock)
        {
          if (_instance == null)
          {
            _instance = new DiagnosticExceptionListener();
          }
        }

        return _instance;
      }
    }

    void IObserver<DiagnosticListener>.OnCompleted()
    {
    }

    void IObserver<KeyValuePair<string, object>>.OnCompleted()
    {
    }

    void IObserver<DiagnosticListener>.OnError(Exception error)
    {
    }

    void IObserver<KeyValuePair<string, object>>.OnError(Exception error)
    {
    }

    void IObserver<DiagnosticListener>.OnNext(DiagnosticListener value)
    {
      if (value.Name == "Microsoft.AspNetCore")
      {
        value.Subscribe(this);
      }
    }

    void IObserver<KeyValuePair<string, object>>.OnNext(KeyValuePair<string, object> value)
    {
      if (value.Key == ExceptionHandlerMiddlewareKey
        || value.Key == DeveloperExceptionPageMiddleware)
      {
        lock (_clientLock)
        {
          if (_currentClient != null)
          {
            var exception = value.Value.GetType().GetTypeInfo().GetDeclaredProperty("exception")?.GetValue(value.Value) as Exception;
            var httpContext = value.Value.GetType().GetTypeInfo().GetDeclaredProperty("httpContext")?.GetValue(value.Value) as HttpContext;
            _currentClient.AutoNotify(exception, httpContext);
          }
        }
      }
    }

    public void ConfigureClient(IClient client)
    {
      lock (_clientLock)
      {
        _currentClient = client;
      }
    }
  }
}
