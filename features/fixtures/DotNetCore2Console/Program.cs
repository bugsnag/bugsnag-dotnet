using System;
using Bugsnag;

namespace DotNetCore2Console
{
  class Program
  {
    static void Main(string[] args)
    {
      var configuration = new Configuration {
        ApiKey = Environment.GetEnvironmentVariable("MAZE_API_KEY"),
        Endpoint = new Uri(Environment.GetEnvironmentVariable("MAZE_ENDPOINT"))
      };
      var bugsnag = new Bugsnag.Client(configuration);
      var app = new Microsoft.Extensions.CommandLineUtils.CommandLineApplication();
      app.Command("handled", config => {
        bugsnag.Notify(new Exception("Handled Error for Maze Runner"));
      });
      app.Execute(args);
    }
  }
}
