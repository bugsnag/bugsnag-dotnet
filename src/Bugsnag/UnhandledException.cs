using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Bugsnag
{
  class UnhandledException
  {
    private static UnhandledException _instance;
    private static readonly object _instanceLock = new object();

    private readonly object _currentClientLock = new object();
    private Client _currentClient;

    private UnhandledException()
    {
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
      lock (_currentClientLock)
      {
        if (_currentClient != null)
        {
          if (e.Exception is Exception ex)
          {
            _currentClient.AutoNotify(ex);
          }
        }
      }
    }

    public static UnhandledException Instance
    {
      get
      {
        lock (_instanceLock)
        {
          if (_instance == null)
          {
            _instance = new UnhandledException();
          }
        }

        return _instance;
      }
    }

    public void ConfigureClient(Client client)
    {
      lock (_currentClientLock)
      {
        _currentClient = client;
      }
    }

    [HandleProcessCorruptedStateExceptions]
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      lock (_currentClientLock)
      {
        if (_currentClient != null)
        {
          if (e.ExceptionObject is Exception ex)
          {
            _currentClient.AutoNotify(ex);
          }
        }
      }
    }
  }
}

#if NET35
namespace System.Threading.Tasks
{
  public class TaskScheduler
  {
    public static event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException { add {} remove {} }
  }

  public class UnobservedTaskExceptionEventArgs : EventArgs
  {
    public Exception Exception { get; set; }
  }
}
#endif

#if NETSTANDARD1_3 || NET35
namespace System.Runtime.ExceptionServices
{
  public class HandleProcessCorruptedStateExceptionsAttribute : Attribute {}
}
#endif

#if NETSTANDARD1_3
namespace System
{
  public class AppDomain
  {
    private static readonly AppDomain _dummyAppDomain = new AppDomain();

    public static AppDomain CurrentDomain
    {
      get
      {
        return _dummyAppDomain;
      }
    }

    public event UnhandledExceptionEventHandler UnhandledException { add {} remove {} }

    public delegate void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs args);
  }

  public class UnhandledExceptionEventArgs : EventArgs
  {
    public Exception ExceptionObject { get; set; }
  }
}
#endif
