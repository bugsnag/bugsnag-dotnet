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
        private static Action<Exception, bool?> NotifyOnUnhandledException = null;

        public static void InstallDefaultHandler(Action<Exception, bool?> actionOnExp)
        {
            if (DefaultHandler != null || NotifyOnUnhandledException != null)
                throw new InvalidOperationException("Default App Domain Handler has already been installed");

            DefaultHandler = new UnhandledExceptionEventHandler(NotifyExceptionHandler);
            NotifyOnUnhandledException = actionOnExp;
            AppDomain.CurrentDomain.UnhandledException += DefaultHandler;
        }

        private static void NotifyExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // TODO : Record and feedback that the runtime is exiting
            var exp = e.ExceptionObject as Exception;
            if (exp != null)
                NotifyOnUnhandledException(exp, e.IsTerminating);
        }

    }
}
