using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Bugsnag
{
    public static class ExceptionHandler
    {
        private static UnhandledExceptionEventHandler DefaultHandler = null;
        private static EventHandler<UnobservedTaskExceptionEventArgs> DefaultTaskHandler = null;
        private static Action<Exception, bool> NotifyOnUnhandledException = null;

        public static void InstallDefaultHandler(Action<Exception, bool> actionOnExp)
        {
            if (DefaultHandler != null || DefaultTaskHandler != null || NotifyOnUnhandledException != null)
                UninstallDefaultHandler();

            DefaultHandler = NotifyExceptionHandler;
            DefaultTaskHandler = NotifyExceptionHandler;
            NotifyOnUnhandledException = actionOnExp;
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

        public static void NotifyExceptionHandler(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var exp = e.Exception as Exception;
            if (exp != null)
                NotifyOnUnhandledException(exp, false);
        }

        private static void NotifyExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var exp = e.ExceptionObject as Exception;
            if (exp != null)
                NotifyOnUnhandledException(exp, e.IsTerminating);
        }
    }
}
