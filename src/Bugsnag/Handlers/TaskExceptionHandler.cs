using System;
using System.Threading.Tasks;

namespace Bugsnag.Handlers
{
    /// <summary>
    /// Responsible for installing and uninstalling task exception handlers for handling application 
    /// level uncaught exceptions
    /// </summary>
    internal class TaskExceptionHandler
    {
        /// <summary>
        /// Holds a reference to the event handler to run if an unobserved exception
        /// occurs in a task.
        /// </summary>
        private EventHandler<UnobservedTaskExceptionEventArgs> taskHandler = null;

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
            if (taskHandler != null || runOnUnhandledException != null)
                UninstallHandler();

            // Set up references to the handlers
            runOnUnhandledException = actionOnException;
            taskHandler = NotifyExceptionHandler;

            // Hook up the handlers to the approriate events
            TaskScheduler.UnobservedTaskException += taskHandler;
        }

        /// <summary>
        /// Removes any handler setup to run when an uncaught exception is detected
        /// </summary>
        public void UninstallHandler()
        {
            // Unhook the task exception handler, if there is one
            if (taskHandler != null)
            {
                TaskScheduler.UnobservedTaskException -= taskHandler;
                taskHandler = null;
            }

            // Clear the notify action
            runOnUnhandledException = null;
        }

        /// <summary>
        /// The default handler to run for unobserved task exceptions.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The unobserved task event arguments</param>
        private void NotifyExceptionHandler(object sender, UnobservedTaskExceptionEventArgs args)
        {
            var exp = args.Exception as Exception;
            if (exp != null)
                runOnUnhandledException(exp, false);
        }
    }
}
