using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Bugsnag.Core
{
    public static class ExceptionHandler
    {
        private static UnhandledExceptionEventHandler DefaultHandler = null;
        private static EventHandler<UnobservedTaskExceptionEventArgs> DefaultTaskHandler = null;
        private static Action<Exception, bool> NotifyOnUnhandledException = null;

        public static void InstallDefaultHandler(Action<Exception, bool> actionOnException)
        {
            if (DefaultHandler != null || DefaultTaskHandler != null || NotifyOnUnhandledException != null)
                UninstallDefaultHandler();

            DefaultHandler = NotifyExceptionHandler;
            DefaultTaskHandler = NotifyExceptionHandler;
            NotifyOnUnhandledException = actionOnException;
            AppDomain.CurrentDomain.UnhandledException += DefaultHandler;
            TaskScheduler.UnobservedTaskException += DefaultTaskHandler;
        }

        public static void UninstallDefaultHandler()
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

        private static void NotifyExceptionHandler(object sender, UnobservedTaskExceptionEventArgs args)
        {
            var exp = args.Exception as Exception;
            if (exp != null)
                NotifyOnUnhandledException(exp, false);
        }

        [HandleProcessCorruptedStateExceptions]
        private static void NotifyExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var exp = args.ExceptionObject as Exception;
            if (exp != null)
                NotifyOnUnhandledException(exp, args.IsTerminating);
        }
    }
}
