.NET 3.5 Console Example Application
====

### Requirements

see the main [README](../#requirements)

### Setup

In the `App.Config` file on line 6 enter the Bugsnag API key that you want to
use with this application:

```
<bugsnag apiKey="APIKEY" releaseStage="development" notifyReleaseStages="development" projectNamespaces="net35_console" projectRoots="C:\app\" />
```

Follow the [instructions](../) in order to bootstrap and build the
required docker images.

Run `docker-compose up` in this directory to start the console application. An
unhandled exception will be triggered.

If you make any code changes you will need to rebuild the docker image
(`docker-compose build`) as we are not mounting this code directory inside of
the container.

The steps taken to configure Bugsnag for this application:
- Add the required nuget packages (see `packages.config`)
- Add the configuration sections to the `App.config`

```
<configSections>
  <section name="bugsnag" type="Bugsnag.ConfigurationSection.Configuration, Bugsnag.ConfigurationSection" />
</configSections>
```

- Create a Bugsnag client when the console application starts

```
var client = new Client(Bugsnag.ConfigurationSection.Configuration.Settings);
```
