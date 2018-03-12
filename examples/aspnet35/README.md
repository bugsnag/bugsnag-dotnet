ASP.NET 3.5 Example Application
====

### Requirements

- Visual Studio 2017
- [.NET framework 3.5](https://docs.microsoft.com/en-us/dotnet/framework/install/dotnet-35-windows-10)

### Setup

In the `Web.Config` file on line 18 enter the Bugsnag API key that you want to
use with this application:

`<bugsnag apiKey="APIKEY" releaseStage="development" notifyReleaseStages="development" projectNamespaces="Bugsnag.Sample.AspNet35" projectRoots="C:\app\" />`

From within Visual Studio you can compile and start the website.

### Steps taken to install Bugsnag

- Add the required nuget packages (see `packages.config`)
- Add the configuration sections to the `Web.config`

```
<configSections>
  <section name="bugsnag" type="Bugsnag.ConfigurationSection.Configuration, Bugsnag.ConfigurationSection" />
</configSections>
```

- Add the http module to the `Web.config`

```
<httpModules>
  <add name="Bugsnag" type="Bugsnag.AspNet.HttpModule, Bugsnag.AspNet" />
</httpModules>
<system.webServer>
  <modules runAllManagedModulesForAllRequests="true">
    <remove name="Bugsnag" />
    <add name="Bugsnag" type="Bugsnag.AspNet.HttpModule, Bugsnag.AspNet" />
  </modules>
</system.webServer>
```
