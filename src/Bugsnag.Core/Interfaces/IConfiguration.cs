using System;

namespace Bugsnag.Core
{
    public interface IConfiguration
    {
        bool IsNotifyReleaseStage();
        Func<Event, bool> BeforeNotifyFunc { get; set; }
        bool IsClassToIgnore(string className);

        string ApiKey { get; }
        string AppVersion { get; set; }
        string ReleaseStage { get; set; }

        string UserId { get; }
        string UserEmail { get; }
        string UserName { get; }
        string LoggedOnUser { get; }
        void SetUser(string userId, string userEmail, string userName);

        string Context { get; }

        Metadata StaticData { get; }

        void SetFilePrefix(params string[] prefixes);
        string RemoveFileNamePrefix(string fileName);

        bool AutoDetectInProject { get; set; }
        bool IsInProjectNamespace(string fullMethodName);
        Uri FinalUrl { get; }
    }
}
