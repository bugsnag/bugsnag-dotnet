using System;

#if !NET35
// Tasks for Async versions of Notify()
using System.Threading.Tasks;
#endif

namespace Bugsnag.Clients
{
    public static class SingletonBaseClient
    {
        /// <summary>
        /// Gets the static Client if configured.
        /// </summary>
        public static BaseClient Client { get; set; }
        public static Configuration Config
        {
            get
            {
                if (Client != null)
                {
                    return Client.Config;
                }
                return null;
            }
        }

        /// <summary>
        /// Notifies Bugsnag of an exception
        /// </summary>
        /// <param name="exception">The exception to send to Bugsnag</param>
        public static void Notify(Exception exception)
        {
            Client.Notify(exception);
        }

        /// <summary>
        /// Notifies Bugsnag of an exception, with an associated severity level
        /// </summary>
        /// <param name="exception">The exception to send to Bugsnag</param>
        /// <param name="severity">The associated severity of the exception</param>
        public static void Notify(Exception exception, Severity severity)
        {
            Client.Notify(exception, severity);
        }

        /// <summary>
        /// Notifies Bugsnag of an exception with associated meta data
        /// </summary>
        /// <param name="exception">The exception to send to Bugsnag</param>
        /// <param name="data">The metadata to send with the exception</param>
        public static void Notify(Exception exception, Metadata data)
        {
            Client.Notify(exception, data);
        }

        /// <summary>
        /// Notifies Bugsnag of an exception, with an associated severity level and meta data
        /// </summary>
        /// <param name="exception">The exception to send to Bugsnag</param>
        /// <param name="severity">The associated severity of the exception</param>
        /// <param name="data">The metadata to send with the exception</param>
        public static void Notify(Exception exception, Severity severity, Metadata data)
        {
            Client.Notify(exception, severity, data);
        }

        /// <summary>
        /// Notifies Bugsnag of an error event
        /// </summary>
        /// <param name="errorEvent">The event to report on</param>
        public static void Notify(Event errorEvent)
        {
            Client.Notify(errorEvent);
        }

#if !NET35
        // Async variants of Notify() methods above
        public static Task NotifyAsync(Exception error)
        {
            return Client.NotifyAsync(error);
        }

        public static Task NotifyAsync(Exception error, Severity severity)
        {
            return Client.NotifyAsync(error, severity);
        }

        public static Task NotifyAsync(Exception error, Metadata data)
        {
            return Client.NotifyAsync(error, data);
        }

        public static Task NotifyAsync(Exception error, Severity severity, Metadata data)
        {
            return Client.NotifyAsync(error, severity, data);
        }
#endif
    }
}
