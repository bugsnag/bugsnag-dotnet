using System;
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
