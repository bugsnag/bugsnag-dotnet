ASP.NET 4.5 MVC & WebApi Example Application
====

### Requirements

- Visual Studio 2017
- .NET Framework 4.5

### Setup

In the `Web.Config` file on line 18 enter the Bugsnag API key that you want to
use with this application:

`<bugsnag apiKey="APIKEY" releaseStage="development" notifyReleaseStages="development" projectNamespaces="Bugsnag.Sample.AspNet35" projectRoots="C:\app\" />`

From within Visual Studio you can compile and start the website.

If you navigate to `About` you will cause an unhandled
exception in MVC. If you hit the API on `http://localhost:8081/api/values` you
will cause an unhandled exception in WebAPI.

### Steps taken to install Bugsnag

- Add the required nuget packages (see `packages.config`)
- Add the configuration sections to the `web.config`

```
<configSections>
  <section name="bugsnag" type="Bugsnag.ConfigurationSection.Configuration, Bugsnag.ConfigurationSection" />
</configSections>
```

- In `App_Start\WebApiConfig.cs` add Bugsnag

```
config.UseBugsnag(Bugsnag.ConfigurationSection.Configuration.Settings);
```
