using System;

namespace Bugsnag.Core
{
    /// <summary>
    /// Defines the interface for setting and retrieving the configuration for a sending notifications to Bugsnag
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets the API key used to send notifications to a specific Bugsnag account
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Gets or sets the application version
        /// </summary>
        string AppVersion { get; set; }

        /// <summary>
        /// Gets or sets the release stage of the application
        /// </summary>
        string ReleaseStage { get; set; }

        /// <summary>
        /// Specifies the release stages that notifications should be sent
        /// </summary>
        /// <param name="releaseStages">The stages to notify on</param>
        void SetNotifyReleaseStages(params string[] releaseStages);

        /// <summary>
        /// Resets any restrictions and notifies on all release stages
        /// </summary>
        void NotifyOnAllReleaseStages();

        /// <summary>
        /// Detects if the current release stage is a stage that should be notified on
        /// </summary>
        /// <returns>True if we should notify, otherwise false</returns>
        bool IsNotifyReleaseStage();

        /// <summary>
        /// Gets or sets the context to apply to the subsequent notifications
        /// </summary>
        string Context { get; set; }

        /// <summary>
        /// Gets or sets the endpoint that defines where to send the notifications
        /// </summary>
        string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the notifications should be sent using SSL
        /// </summary>
        bool UseSsl { get; set; }

        Func<Event, bool> BeforeNotifyFunc { get; set; }

        bool IsClassToIgnore(string className);

        string UserId { get; }

        string UserEmail { get; }

        string UserName { get; }

        string LoggedOnUser { get; }

        void SetUser(string userId, string userEmail, string userName);

        Metadata StaticData { get; }

        void SetFilePrefix(params string[] prefixes);

        string RemoveFileNamePrefix(string fileName);

        bool AutoDetectInProject { get; set; }

        bool IsInProjectNamespace(string fullMethodName);

        Uri FinalUrl { get; }
    }
}
