using Bugsnag.Message.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bugsnag
{
    public class Configuration
    {
        private const string DefaultEndpoint = "notify.bugsnag.com";

        private const string NotifierName = ".NET Bugsnag Notifier (ALPHA)";
        private const string NotifierVersion = "1.2.6";
        private const string NotifierUrl = "https://bugsnag.com";

        public static readonly NotifierInfo NotifierInfo = new NotifierInfo
        {
            Name = ".NET Bugsnag Notifier (ALPHA)",
            Version = "1.2.6",
            Url = "https://bugsnag.com"
        };

        public string ApiKey { get; private set; }


        private bool autoNotify = true;
        private bool useSSL = false;
        private string endpoint;
        private List<string> notifyReleaseStages = null;
        private List<string> filters = new List<string> { "password" };
        //private List<string> projectPackages;
        //private List<string> ignoreClasses;

        public string AppVersion
        {
            get { return "0.1.2"; }
        }

        public string ReleaseStage
        {
            get { return "Pre Stage BETA"; }
        }

        public string OsVersion { get; private set; }

        public Configuration(string apiKey)
        {
            ApiKey = apiKey;
            OsVersion = Environment.OSVersion.ToString();
        }


    }
}
