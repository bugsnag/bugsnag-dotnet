using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugsnagDemoConsole
{
    public class Class2
    {
        public static void GetExp()
        {
            try
            {
                Class3.GetExp();
            }
            catch (Exception exp)
            {
                exp.HelpLink = "http://msdn.microsoft.com/en-us/library/system.dividebyzeroexception(v=vs.110).aspx";
                throw new ArithmeticException("Can't Multiply 1 and 1", exp);
            }
        }
    }
}
