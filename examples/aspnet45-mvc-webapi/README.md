ASP.NET 4.5 MVC & WebApi Example Application
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
`http://localhost:8081/`. If you navigate to `About` you will cause an unhandled
exception in MVC. If you hit the API on `http://localhost:8081/api/values` you
will cause an unhandled exception in WebAPI.

If you make any code changes you will need to rebuild the docker image
(`docker-compose build`) as we are not mounting this code directory inside of
the container.

The steps taken to configure Bugsnag for this application:
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
