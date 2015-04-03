using System;

#if !NET35
using System.Runtime.ExceptionServices;
#endif

namespace Bugsnag.Handlers
{
    /// <summary>
    /// Responsible for installing and uninstalling unobserved exception handler for handling application 
    /// level uncaught exceptions
    /// </summary>
    internal class UnhandledExceptionHandler
    {
        /// <summary>
        /// Holds a reference to the handler to run when an unhandled exception
        /// occurs in the current app domain.
        /// </summary>
        private UnhandledExceptionEventHandler appDomainHandler = null;

        /// <summary>
        /// The action to run when handling exceptions
        /// </summary>
        private Action<Exception, bool> runOnUnhandledException = null;

        /// <summary>
        /// Hooks up a handler to execute when an uncaught exception is detected
        /// </summary>
        /// <param name="actionOnException">The action to execute, taking as parameters the uncaught exception
        /// and a flag indicating if the runtime is ending</param>
        public void InstallHandler(Action<Exception, bool> actionOnException)
        {
            // If any existing handlers are installed, remove them first
            if (appDomainHandler != null || runOnUnhandledException != null)
                UninstallHandler();

            // Set up references to the handlers
            runOnUnhandledException = actionOnException;
            appDomainHandler = NotifyExceptionHandler;

            // Hook up the handlers to the approriate events
            AppDomain.CurrentDomain.UnhandledException += appDomainHandler;
        }

        /// <summary>
        /// Removes any handler setup to run when an uncaught exception is detected
        /// </summary>
        public void UninstallHandler()
        {
            // Unhook the app domain exception handler, if there is one
            if (appDomainHandler != null)
            {
                AppDomain.CurrentDomain.UnhandledException -= appDomainHandler;
                appDomainHandler = null;
            }

            // Clear the notify action
            runOnUnhandledException = null;
        }

        /// <summary>
        /// The default handler to run for uncaught app domain exceptions. Will be run on corrupted
        /// state exceptions
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The unhandled exception event arguments</param>
#if !NET35
        [HandleProcessCorruptedStateExceptions]
#endif
        private void NotifyExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var exp = args.ExceptionObject as Exception;
            if (exp != null)
                runOnUnhandledException(exp, args.IsTerminating);
        }
    }
}
