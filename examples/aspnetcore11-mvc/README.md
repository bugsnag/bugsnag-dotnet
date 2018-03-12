ASP.NET Core 1.1 MVC Example Application
====

### Requirements

- Visual Studio 2017
- .NET Core 1.1 development tools for web

### Setup

In the `Startup.cs` file enter the Bugsnag API key that you want to use with
this application:

```
services.AddBugsnag(config => {
    config.ApiKey = "<API_KEY>";
    config.ProjectNamespaces = new[]{ "aspnetcore11_mvc" };
    config.ProjectRoots = new[]{ @"C:\app\" };
});
```

From within Visual Studio you can compile and start the website.

### Steps taken to install Bugsnag

- Add the required nuget packages (see `aspnetcore11-mvc.csproj`)
- Add Bugsnag with the convenience method in `Startup.cs`
