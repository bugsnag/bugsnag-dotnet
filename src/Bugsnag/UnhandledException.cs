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
    private IClient _currentClient;

    private UnhandledException()
    {
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
      TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
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

    public void ConfigureClient(IClient client, IConfiguration configuration)
    {
      if (configuration.AutoNotify)
      {
        lock (_currentClientLock)
        {
          _currentClient = client;
        }
      }
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
      HandleEvent(null, true);
    }

    private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
      // Starting with .NET 4.5, this does not kill the process:
      // https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler.unobservedtaskexception
      HandleEvent(e.Exception as Exception, false);
    }

    [HandleProcessCorruptedStateExceptions]
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      HandleEvent(e.ExceptionObject as Exception, e.IsTerminating);
    }

    private void HandleEvent(Exception exception, bool runtimeEnding)
    {
      if (exception != null)
      {
        lock (_currentClientLock)
        {
          if (_currentClient != null)
          {
            if (_currentClient.Configuration.AutoNotify)
            {
              _currentClient.Notify(exception, Payload.HandledState.ForUnhandledException());
            }
          }
        }
      }

      if (runtimeEnding)
      {
        SessionsStore.Instance.Stop();
        ThreadQueueDelivery.Instance.Stop();
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

    public bool Observed { get; set; }
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

    public event EventHandler ProcessExit { add {} remove {} }

    public delegate void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs args);
  }

  public class UnhandledExceptionEventArgs : EventArgs
  {
    public Exception ExceptionObject { get; set; }

    public bool IsTerminating { get; set; }
  }
}
#endif
