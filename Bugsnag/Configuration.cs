using Bugsnag.Event;
using Bugsnag.Message.Core;
using Bugsnag.Message.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bugsnag
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

        public bool UseSsl { get; set; }
        public string[] FilePrefix { get; set; }
        public bool AutoDetectInProject { get; set; }

        public Func<Error,bool> BeforeNotifyFunc { get; set; }

        public MetaData StaticData { get; private set; }

        public Configuration(string apiKey)
        {
            ApiKey = apiKey;
            AppVersion = "1.0.0";
            ReleaseStage = "Development";
            StaticData = new MetaData();
            UseSsl = true;
            AutoDetectInProject = true;
            UserId = Environment.UserName;
            LoggedOnUser = Environment.UserDomainName + "\\" + Environment.UserName;
        }

        public void SetUser(string userId, string userEmail, string userName)
        {
            UserId = userId;
            UserEmail = userEmail;
            UserName = userName;
        }
    }
}
