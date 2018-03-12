.NET 3.5 Console Example Application
====

### Requirements

- Visual Studio 2017
- [.NET framework 3.5](https://docs.microsoft.com/en-us/dotnet/framework/install/dotnet-35-windows-10)

### Setup

In the `App.Config` file on line 6 enter the Bugsnag API key that you want to
use with this application:

```
<bugsnag apiKey="APIKEY" releaseStage="development" notifyReleaseStages="development" projectNamespaces="net35_console" projectRoots="C:\app\" />
```

From within Visual Studio you can compile and launch the console application.

### Steps taken to install Bugsnag

- Add the required nuget packages (see `packages.config`)
- Add the configuration sections to the `App.config`

```
<configSections>
  <section name="bugsnag" type="Bugsnag.ConfigurationSection.Configuration, Bugsnag.ConfigurationSection" />
</configSections>
```

- Create a Bugsnag client when the console application starts

```
var bugsnag = new Client(Bugsnag.ConfigurationSection.Configuration.Settings);
```
