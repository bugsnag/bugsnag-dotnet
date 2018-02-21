using System;
using Bugsnag;

namespace netcore11_console
{
    class Program
    {
        static void Main(string[] args)
        {
          var client = new Client(new Configuration("APIKEY") {
              ProjectRoots = new[] { @"C:\app\" },
              ProjectNamespaces = new[] { "netcore11_console" }
          });

          Console.WriteLine("Hello World!");

          try
          {
              throw new NotImplementedException();
          }
          catch (Exception ex)
          {
              client.Notify(ex);
              throw;
          }
        }
    }
}
