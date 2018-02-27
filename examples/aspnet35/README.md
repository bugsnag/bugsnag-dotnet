ASP.NET 3.5 Example Application
====

### Requirements

see the main [README](../#requirements)

### Setup

In the `Web.Config` file on line 18 enter the Bugsnag API key that you want to
use with this application:

`<bugsnag apiKey="APIKEY" releaseStage="development" notifyReleaseStages="development" projectNamespaces="Bugsnag.Sample.AspNet35" projectRoots="C:\app\" />`

Follow the [instructions](../) in order to bootstrap and build the
required docker images.

Run `docker-compose up` in this directory to start the website. You can browse to
`http://localhost:8080/Index.aspx` to see the sample page and produce both a
handled exception and an unhandled exception.

If you make any code changes you will need to rebuild the docker image
(`docker-compose build`) as we are not mounting this code directory inside of
the container.

The steps taken to configure Bugsnag for this application:
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
