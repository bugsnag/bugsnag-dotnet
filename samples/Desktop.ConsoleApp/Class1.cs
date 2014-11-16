using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugsnagDemoConsole
{
    public class Class1
    {
        public static void GetExp()
        {
            Class2.GetExp();
        }
    }
}
