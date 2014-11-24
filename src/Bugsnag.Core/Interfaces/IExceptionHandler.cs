using System;

namespace Bugsnag.Core
{
    public interface IExceptionHandler
    {
        void InstallHandler(Action<Exception, bool> actionOnException);
        void UninstallHandler();
    }
}
