using Bugsnag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugsnagDemoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var bugsnag = new Client("9134c4469d16f30f025a1e98f45b3ddb");

            bugsnag.Config.AppVersion = "5.5.5";
            bugsnag.Config.ReleaseStage = "Alpha";
            bugsnag.Config.SetUser("1234", "aaaa@bbbb.com", "Aaaa Bbbb");

            bugsnag.Config.StaticData.AddToTab("Random", new { key1 = "Stuff", key2 = "Other Stuff" });
            bugsnag.Config.FilePrefix = new List<string> { @"e:\GitHub\Bugsnag-NET\" };

            bugsnag.Config.BeforeNotifyFunc = error =>
            {
                error.MetaData.AddToTab("CallBack", "Check", true);
                return true;
            };

            bugsnag.Config.AutoDetectInProject = false;
            bugsnag.Config.ProjectNamespaces = new List<string> { "Microsoft.VisualStudio.HostingProcess"};
            //bugsnag.Config.AutoDetectInProject = false;
            //bugsnag.Config.ShowTraces = false;

            //bugsnag.Notify(new ArgumentException("Non-fatal"));
            Class1.GetExp();
        }
    }
}
