using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Bugsnag.Core
{
    public class ExceptionHandler
    {
        private UnhandledExceptionEventHandler DefaultHandler = null;
        private EventHandler<UnobservedTaskExceptionEventArgs> DefaultTaskHandler = null;
        private Action<Exception, bool> NotifyOnUnhandledException = null;

        public void InstallHandler(Action<Exception, bool> actionOnException)
        {
            if (DefaultHandler != null || DefaultTaskHandler != null || NotifyOnUnhandledException != null)
                UninstallHandler();

            DefaultHandler = NotifyExceptionHandler;
            DefaultTaskHandler = NotifyExceptionHandler;
            NotifyOnUnhandledException = actionOnException;
            AppDomain.CurrentDomain.UnhandledException += DefaultHandler;
            TaskScheduler.UnobservedTaskException += DefaultTaskHandler;
        }

        public void UninstallHandler()
        {
            if (DefaultHandler != null)
            {
                AppDomain.CurrentDomain.UnhandledException -= DefaultHandler;
                DefaultHandler = null;
            }

            if (DefaultTaskHandler != null)
            {
                TaskScheduler.UnobservedTaskException -= DefaultTaskHandler;
                DefaultTaskHandler = null;
            }
            NotifyOnUnhandledException = null;
        }

        private void NotifyExceptionHandler(object sender, UnobservedTaskExceptionEventArgs args)
        {
            var exp = args.Exception as Exception;
            if (exp != null)
                NotifyOnUnhandledException(exp, false);
        }

        [HandleProcessCorruptedStateExceptions]
        private void NotifyExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var exp = args.ExceptionObject as Exception;
            if (exp != null)
                NotifyOnUnhandledException(exp, args.IsTerminating);
        }
    }
}
