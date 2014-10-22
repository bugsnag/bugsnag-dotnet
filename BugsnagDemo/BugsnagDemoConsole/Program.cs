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
            Class1.GetExp();
        }
    }
}
