using System;

namespace Bugsnag.Core
{
    /// <summary>
    /// Defines the interface for installing and uninstalling handlers for handling application 
    /// level uncaught exceptions
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Hooks up a handler to execute when an uncaught exception is detected
        /// </summary>
        /// <param name="actionOnException">The action to execute, taking as parameters the uncaught exception
        /// and a flag indicating if the runtime is ending</param>
        void InstallHandler(Action<Exception, bool> actionOnException);

        /// <summary>
        /// Removes any handler setup to run when an uncaught exception is detected
        /// </summary>
        void UninstallHandler();
    }
}
