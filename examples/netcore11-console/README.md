.NET Core 1.1 Example Application
====

### Requirements

- Visual Studio 2017
- .NET Core 1.1 development tools

### Setup

In the `Program.cs` file enter the Bugsnag API key that you want to use with
this application:

```
var bugsnag = new Client(new Configuration("APIKEY") {
    ProjectRoots = new[] { @"C:\app\" },
    ProjectNamespaces = new[] { "netcore11_console" }
});
```

From within Visual Studio you can compile and launch the console application.

### Steps taken to install Bugsnag

- Add the required nuget packages (see `netcore11-console.csproj`)
- Create a Bugsnag client when the console application starts

```
var bugsnag = new Client(new Configuration("APIKEY") {
    ProjectRoots = new[] { @"C:\app\" },
    ProjectNamespaces = new[] { "netcore11_console" }
});
```
