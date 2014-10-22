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
            throw new ArithmeticException("Can't Add 1 and 1");
        }
    }
}
