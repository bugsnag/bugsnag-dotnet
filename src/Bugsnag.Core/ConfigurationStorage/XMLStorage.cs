using System;
using System.Configuration;

namespace Bugsnag.ConfigurationStorage
{
    class XMLStorage : ConfigurationSection, IConfigurationStorage
    {
        private static XMLStorage _storage = ConfigurationManager.GetSection("bugsnagConfig") as XMLStorage;

        public static XMLStorage Settings
        {
            get
            {
                return _storage;
            }
        }

        [ConfigurationProperty("apiKey", IsRequired = true)]
        [RegexStringValidator("^[a-fA-F0-9]{32}$")]
        public string ApiKey
        {
            get { return (String)this["apiKey"]; }
            set { this["apiKey"] = value; }
        }

        [ConfigurationProperty("appVersion", IsRequired = false)]
        [StringValidator]
        public string AppVersion
        {
            get { return (String)this["appVersion"]; }
            set { this["appVersion"] = value; }
        }

        public string ReleaseStage
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Endpoint
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string UserId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string UserEmail
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string UserName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Context
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool AutoDetectInProject
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string[] NotifyReleaseStages
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string[] FilePrefixes
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string[] ProjectNamespaces
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string[] IgnoreClasses
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string[] MetadataFilters
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool AutoNotify
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
