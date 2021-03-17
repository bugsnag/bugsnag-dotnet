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
    private bool _unobservedTerminates;

    private UnhandledException()
    {
      _unobservedTerminates = DetermineUnobservedTerminates();
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

    /// <summary>
    /// Determines if an UnobservedTaskException leads to the process terminating, based on the target
    /// framework and (when applicable) configuration.
    /// </summary>
    /// <returns></returns>
    private bool DetermineUnobservedTerminates()
    {
#if NET35 || NET40
      return true;
#elif NET45
      System.Xml.Linq.XElement configFile = null;
      if(System.IO.File.Exists(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile))
        configFile = System.Xml.Linq.XElement.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile); 

      var configValue = configFile?.Element("runtime")?.Element("ThrowUnobservedTaskExceptions")?.Attribute("enabled")?.Value;
      bool value;
      var success = bool.TryParse(configValue, out value);
      return success && value;
#else //NETSTANDARD1_3 || NETSTANDARD2_0
      return false;
#endif
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
      HandleEvent(null, true);
    }

    private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
      HandleEvent(e.Exception as Exception, _unobservedTerminates && !e.Observed);
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
