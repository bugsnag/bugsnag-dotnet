.NET 4.7 Console Example Application
====

### Requirements

- Visual Studio 2017
- .NET Framework 4.7

### Setup

In the `App.Config` file on line 6 enter the Bugsnag API key that you want to
use with this application:

```
<bugsnag apiKey="APIKEY" releaseStage="development" notifyReleaseStages="development" projectNamespaces="net47_console" projectRoots="C:\app\" />
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
