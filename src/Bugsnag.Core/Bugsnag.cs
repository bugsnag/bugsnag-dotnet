using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bugsnag.Core
{
    public static class BugsnagSingleton
    {
        /// <summary>
        /// Gets the static Client if configured.
        /// </summary>
        public static Client Client { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class. Will use all the default settings and will 
        /// automatically hook into uncaught exceptions.
        /// </summary>
        /// <param name="apiKey">The Bugsnag API key to send notifications with</param>
        public static void Start(string apiKey)
        {
            if (Client == null)
            {
                Client = new Client(apiKey);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class. Provides the option to automatically 
        /// hook into uncaught exceptions. Uses default dependencies.
        /// </summary>
        /// <param name="apiKey">The Bugsnag API key to use</param>
        /// <param name="autoNotify">True if the client will automatically notify uncaught exceptions, otherwise false</param>
        public static void Start(string apiKey, bool autoNotify)
        {
            if (Client == null)
            {
                Client = new Client(apiKey, autoNotify);
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
    }
}
