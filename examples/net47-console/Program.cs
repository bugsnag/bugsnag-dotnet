using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bugsnag;

namespace net47_console
{
  class Program
  {
    static void Main(string[] args)
    {
      var client = new Client(Bugsnag.ConfigurationSection.Configuration.Settings);
      Console.WriteLine("Hello World!");
      throw new NotImplementedException();
    }
  }
}
