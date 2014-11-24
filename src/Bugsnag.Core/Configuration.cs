using System;
using System.Linq;
using System.Collections.Generic;

namespace Bugsnag.Core
{
    public class Configuration
    {
        public string ApiKey { get; private set; }

        public string AppVersion { get; set; }
        public string ReleaseStage { get; set; }

        public string UserId { get; private set; }
        public string UserEmail { get; private set; }
        public string UserName { get; private set; }
        public string LoggedOnUser { get; private set; }

        public string Context { get; set; }


        public bool AutoDetectInProject { get; set; }
        public bool SendThreads { get; set; }

        private List<string> FilePrefix { get; set; }
        private List<string> NotifyReleaseStages { get; set; }
        private List<string> IgnoreClasses { get; set; }
        private List<string> ProjectNamespaces { get; set; }

        public Func<Event, bool> BeforeNotifyFunc { get; set; }

        public Metadata StaticData { get; private set; }

        public bool UseSsl { get; set; }
        public string Endpoint { get; private set; }
        public Uri FinalUrl
        {
            get { return new Uri((UseSsl ? @"https:\\" : "http\\") + Endpoint); }
        }

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
            SendThreads = false;
            FilePrefix = new List<string>();
            IgnoreClasses = new List<string>();
            ProjectNamespaces = new List<string>();
            NotifyReleaseStages = null;
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
            FilePrefix.ForEach(x => result = result.Replace(x, ""));
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

        public void SetNotifyReleaseStages(params string[] releaseStages)
        {
            NotifyReleaseStages = releaseStages.ToList();
        }

        public void NotifyOnAllReleaseStages()
        {
            NotifyReleaseStages = null;
        }

        public bool IsNotifyReleaseStage()
        {
            if (NotifyReleaseStages == null)
                return true;

            return NotifyReleaseStages.Any(x => x == ReleaseStage);
        }
    }
}
