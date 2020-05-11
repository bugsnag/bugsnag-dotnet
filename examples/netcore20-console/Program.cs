using System;
using Bugsnag;

namespace netcore20_console
{
  class Program
  {
    static void Main(string[] args)
    {
      var client = new Client(new Configuration("APIKEY")
      {
        ProjectRoots = new[] { @"C:\app\" },
        ProjectNamespaces = new[] { "netcore20_console" }
      });

      Console.WriteLine("Hello World!");

      throw new NotImplementedException();
    }
  }
}
