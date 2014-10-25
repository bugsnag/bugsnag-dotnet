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
            var client = new Client("9134c4469d16f30f025a1e98f45b3ddb");

            client.Config.AppVersion = "5.5.5";
            client.Config.ReleaseStage = "Alpha";
            client.Config.SetUser("1234", "aaaa@bbbb.com", "Aaaa Bbbb");

            client.Config.StaticData.AddToTab("Random", new { key1 = "Stuff", key2 = "Other Stuff" });


            Class1.GetExp();
        }
    }
}
