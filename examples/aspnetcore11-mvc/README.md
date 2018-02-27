ASP.NET Core 1.1 MVC Example Application
====

### Requirements

see the main [README](../#requirements)

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

Follow the [instructions](../) in order to bootstrap and build the
required docker images.

Run `docker-compose up` in this directory to start the website. You can browse to
`http://localhost:8082/`. If you navigate to `About` you will cause an unhandled
exception in MVC.

If you make any code changes you will need to rebuild the docker image
(`docker-compose build`) as we are not mounting this code directory inside of
the container.

The steps taken to configure Bugsnag for this application:
- Add the required nuget packages (see `aspnetcore11-mvc.csproj`)
- Add Bugsnag with the convenience method in `Startup.cs`
