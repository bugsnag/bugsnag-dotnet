using System;
using Bugsnag.Clients;

#if !NET35
// Tasks for Async versions of Notify()
using System.Threading.Tasks;
#endif

namespace Bugsnag
{
    /// <summary>
    /// Defines extensions to simplify sending of notifications to Bugsnag
    /// </summary>
    public static class ErrorExtensions
    {

        // Defines the current type of client
        public static ClientTypes ClientType = ClientTypes.Standard;
        
        /// <summary>
        /// Constructor for extensions to define default settings
        /// </summary>
        static ErrorExtensions()
        {
            if (System.Web.HttpContext.Current != null && ClientType == ClientTypes.Standard)
                ClientType = ClientTypes.WebMvc; // Default to web mvc if http context available
            SingletonBaseClient.Client = new BaseClient(); // Use default base client (appSetting or custom config)
        }

        /// <summary>
        /// Gets the base client based on selected configuration.
        /// </summary>
        private static BaseClient Client
        {
            get
            {
                switch (ClientType)
                {
                    #if !NET35
                    case ClientTypes.WebApi:
                        return WebAPIClient.Client;
                    #endif
                    case ClientTypes.WebMvc:
                        return WebMVCClient.Client;
                    case ClientTypes.Wpf:
                        return WPFClient.Client;
                    default: // Standard
                        return SingletonBaseClient.Client;
                }
            }
        }

        /// <summary>
        /// Notifies the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        public static void Notify(this Exception error)
        {
            Client.Notify(error);
        }

        /// <summary>
        /// Notifies the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="metadata">The metadata.</param>
        public static void Notify(this Exception error, Metadata metadata)
        {
            Client.Notify(error, metadata);
        }

        /// <summary>
        /// Notifies the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="severity">The severity.</param>
        public static void Notify(this Exception error, Severity severity)
        {
            Client.Notify(error, severity);
        }

        /// <summary>
        /// Notifies the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="metadata">The metadata.</param>
        public static void Notify(this Exception error, Severity severity, Metadata metadata)
        {
            Client.Notify(error, severity, metadata);
        }

#if !NET35

        /// <summary>
        /// Notifies the async.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>task</returns>
        public static Task NotifyAsync(this Exception error)
        {
            return Client.NotifyAsync(error);
        }

        /// <summary>
        /// Notifies the async.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>task</returns>
        public static Task NotifyAsync(this Exception error, Metadata metadata)
        {
            return Client.NotifyAsync(error, metadata);
        }

        /// <summary>
        /// Notifies the async.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="severity">The severity.</param>
        /// <returns>task</returns>
        public static Task NotifyAsync(this Exception error, Severity severity)
        {
            return Client.NotifyAsync(error, severity);
        }

        /// <summary>
        /// Notifies the async.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>task</returns>
        public static Task NotifyAsync(this Exception error, Severity severity, Metadata metadata)
        {
            return Client.NotifyAsync(error, severity, metadata);
        }

#endif

    }
}
