.NET Core 1.1 Example Application
====

### Requirements

see the main [README](../#requirements)

### Setup

In the `Program.cs` file enter the Bugsnag API key that you want to use with
this application:

```
var client = new Client(new Configuration("APIKEY") {
    ProjectRoots = new[] { @"C:\app\" },
    ProjectNamespaces = new[] { "netcore11_console" }
});
```

Follow the [instructions](../) in order to bootstrap and build the
required docker images.

If you make any code changes you will need to rebuild the docker image
(`docker-compose build`) as we are not mounting this code directory inside of
the container.

The steps taken to configure Bugsnag for this application:
- Add the required nuget packages (see `netcore11-console.csproj`)

- Create a Bugsnag client when the console application starts

```
var client = new Client(new Configuration("APIKEY") {
    ProjectRoots = new[] { @"C:\app\" },
    ProjectNamespaces = new[] { "netcore11_console" }
});
```
