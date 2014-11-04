using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bugsnag
{
    public static class ExceptionHandler
    {
        private static UnhandledExceptionEventHandler DefaultHandler = null;
        private static Action<Exception, bool> NotifyOnUnhandledException = null;

        public static void InstallDefaultHandler(Action<Exception, bool> actionOnExp)
        {
            if (DefaultHandler != null || NotifyOnUnhandledException != null)
                UninstallDefaultHandler();

            DefaultHandler = new UnhandledExceptionEventHandler(NotifyExceptionHandler);
            NotifyOnUnhandledException = actionOnExp;
            AppDomain.CurrentDomain.UnhandledException += DefaultHandler;
        }

        public static void UninstallDefaultHandler()
        {
            if (DefaultHandler != null)
            {
                AppDomain.CurrentDomain.UnhandledException -= DefaultHandler;
                DefaultHandler = null;
            }
            NotifyOnUnhandledException = null;
        }

        private static void NotifyExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var exp = e.ExceptionObject as Exception;
            if (exp != null)
                NotifyOnUnhandledException(exp, e.IsTerminating);
        }

    }
}
