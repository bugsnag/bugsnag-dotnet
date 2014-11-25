using System;
using System.Collections.Generic;
using System.Linq;

namespace Bugsnag.Core
{
    /// <summary>
    /// Defines the configuration for a sending notifications to Bugsnag
    /// </summary>
    public class Configuration : IConfiguration
    {
        /// <summary>
        /// Gets the API key used to send notifications to a specific Bugsnag account
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Gets or sets the application version
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// Gets or sets the release stage of the application
        /// </summary>
        public string ReleaseStage { get; set; }

        /// <summary>
        /// Gets or sets the context to apply to the subsequent notifications
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets the endpoint that defines where to send the notifications
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the notifications should be sent using SSL
        /// </summary>
        public bool UseSsl { get; set; }

        public string UserId { get; private set; }

        public string UserEmail { get; private set; }

        public string UserName { get; private set; }

        public string LoggedOnUser { get; private set; }

        public bool AutoDetectInProject { get; set; }

        public Func<Event, bool> BeforeNotifyFunc { get; set; }

        public Metadata StaticData { get; private set; }

        public Uri FinalUrl
        {
            get { return new Uri((UseSsl ? @"https://" : "http://") + Endpoint); }
        }

        private List<string> NotifyReleaseStages { get; set; }

        private List<string> FilePrefix { get; set; }

        private List<string> IgnoreClasses { get; set; }

        private List<string> ProjectNamespaces { get; set; }

        public Configuration(string apiKey)
        {
            ApiKey = apiKey;
            AppVersion = "1.0.0";
            ReleaseStage = "Development";
            StaticData = new Metadata();
            UseSsl = true;
            AutoDetectInProject = true;
            UserId = Environment.UserName;
            LoggedOnUser = Environment.UserDomainName + "\\" + Environment.UserName;
            Context = null;
            Endpoint = "notify.bugsnag.com";
            FilePrefix = new List<string>();
            IgnoreClasses = new List<string>();
            ProjectNamespaces = new List<string>();
            NotifyReleaseStages = null;
        }

        /// <summary>
        /// Specifies the release stages that notifications should be sent
        /// </summary>
        /// <param name="releaseStages">The stages to notify on</param>
        public void SetNotifyReleaseStages(params string[] releaseStages)
        {
            NotifyReleaseStages = releaseStages.ToList();
        }

        /// <summary>
        /// Resets any restrictions and notifies on all release stages
        /// </summary>
        public void NotifyOnAllReleaseStages()
        {
            NotifyReleaseStages = null;
        }

        /// <summary>
        /// Detects if the current release stage is a stage that should be notified on
        /// </summary>
        /// <returns>True if we should notify, otherwise false</returns>
        public bool IsNotifyReleaseStage()
        {
            // Notify if no restrictions have been set or the release stage hasn't been set
            if (NotifyReleaseStages == null || ReleaseStage == null)
                return true;

            return NotifyReleaseStages.Any(x => x == ReleaseStage);
        }

        public void SetUser(string userId, string userEmail, string userName)
        {
            UserId = userId;
            UserEmail = userEmail;
            UserName = userName;
        }

        public void SetFilePrefix(params string[] prefixes)
        {
            FilePrefix = prefixes.ToList();
        }

        public string RemoveFileNamePrefix(string fileName)
        {
            var result = fileName;

            if (result == null)
                return result;
            FilePrefix.ForEach(x => result = result.Replace(x, string.Empty));
            return result;
        }

        public void SetIgnoreClasses(params string[] classNames)
        {
            IgnoreClasses = classNames.ToList();
        }

        public bool IsClassToIgnore(string className)
        {
            return IgnoreClasses.Contains(className);
        }

        public void SetProjectNamespaces(params string[] projectNamespaces)
        {
            ProjectNamespaces = projectNamespaces.ToList();
        }

        public bool IsInProjectNamespace(string fullMethodName)
        {
            return ProjectNamespaces.Any(x => fullMethodName.StartsWith(x, StringComparison.Ordinal));
        }
    }
}
