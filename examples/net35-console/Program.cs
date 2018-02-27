using System;
using Bugsnag;

namespace net35_console
{
  class Program
  {
    static void Main(string[] args)
    {
      var client = new Client(Bugsnag.ConfigurationSection.Configuration.Settings);

      Console.WriteLine("Hello World!");

      try
      {
          throw new NotImplementedException();
      }
      catch (Exception ex)
      {
          client.Notify(ex);
      }
    }
  }
}
