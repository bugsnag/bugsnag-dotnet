using Bugsnag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

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

            bugsnag.Config.SendThreads = true;

            bugsnag.Config.BeforeNotifyFunc = error =>
            {
                error.MetaData.AddToTab("CallBack", "Check", true);
                return true;
            };

            // UNOBSERVED TASK EXCEPTION
            //var t = Task.Factory.StartNew(() =>
            //{
            //    Thread.Sleep(1000);
            //    throw new ArgumentOutOfRangeException("Thread Exp");
            //});
            //Thread.Sleep(2000);
            //t = null;
            //GC.Collect();

            // NORMAL CALL STACK EXCEPTION
            //Class1.GetExp();

            // ACCESS VIOLATION EXCEPTION
            //IntPtr ptr = new IntPtr(1000);
            //System.Runtime.InteropServices.Marshal.StructureToPtr(1000, ptr, true);

            // MULTIPLE THREADS EXCEPTION
            for(int i = 0; i< 5 ; i++)
            {
                Task.Factory.StartNew(ConsoleWork);
            }
            Thread.Sleep(1000);
            throw new RankException("Wrong Rank with 5 threads");
        }

        private static void ConsoleWork()
        {
            Thread.Sleep(5000);
        }
    }
}
