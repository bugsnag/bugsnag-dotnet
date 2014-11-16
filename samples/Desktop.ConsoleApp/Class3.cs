using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugsnagDemoConsole
{
    public class Class3
    {
        public static void GetExp()
        {
            var isZero = 1 - 1;

            var incorrect = 1 / isZero;
        }
    }
}
