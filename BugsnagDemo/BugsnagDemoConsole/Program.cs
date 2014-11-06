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

            var t = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(1000);
                throw new ArgumentOutOfRangeException("Thread Exp");
            });
            t.Wait();

            //bugsnag.Notify(new ArgumentException("Non-fatal"));
            //Class1.GetExp();
            System.Threading.Thread.Sleep(5000);
            t = null;
            GC.Collect();
            System.Threading.Thread.Sleep(5000);

            //Recursive(0);
        }

        static void Recursive(int value)
        {
            // Write call number and call this method again.
            // ... The stack will eventually overflow.
         
            Recursive(++value);
        }
    }
}
