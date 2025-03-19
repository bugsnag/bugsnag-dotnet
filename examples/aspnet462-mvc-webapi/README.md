ASP.NET 4.6.2 MVC & WebApi Example Application
====

### Requirements

- Visual Studio 2022
- .NET Framework 4.6.2+

### Setup

The example project is configured to install Bugsnag dependencies from a local package source. Run the build script from the repository root to build and generate the local packages:

```
.\build.ps1 --Target Pack
```

Or, alternatively, you can use Visual Studio to install the latest version from `nuget.org`

In the `Web.Config` file enter the Bugsnag API key that you want to
use with this application:

```xml
<bugsnag apiKey="YOUR_API_KEY" releaseStage="development" notifyReleaseStages="development" projectNamespaces="aspnet462_mvc_webapi" />
```

### Run the App

Compile and start the website from Visual Studio.

- Navigate to `/Home/About/` to send a handled exception
- Navigate to `/Home/Crash/` to trigger an unhandled exception in MVC
- Hit the API on `/api/values` to trigger an unhandled exception in Web API

### Steps taken to install Bugsnag

- Add the required nuget packages (see `packages.config`)
- Add the configuration sections to the `web.config`

```xml
<configSections>
  <section name="bugsnag" type="Bugsnag.ConfigurationSection.Configuration, Bugsnag.ConfigurationSection" />
</configSections>
```

- In `App_Start\WebApiConfig.cs` add Bugsnag

```csharp
config.UseBugsnag(Bugsnag.ConfigurationSection.Configuration.Settings);
```
