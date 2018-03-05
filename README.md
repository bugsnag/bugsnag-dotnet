# Bugsnag Notifier for .NET

[![build status](https://ci.appveyor.com/api/projects/status/0c0mlvop5equtm43/branch/master?svg=true)](https://ci.appveyor.com/project/snmaynard/bugsnag-dotnet)
[![Documentation](https://img.shields.io/badge/documentation-latest-blue.svg)](https://docs.bugsnag.com/platforms/dotnet/)

The Bugsnag Notifier for .NET gives you instant notification of exceptions
thrown from your .NET apps. Any uncaught exceptions will trigger a
notification to be sent to your Bugsnag project.

[Bugsnag](https://www.bugsnag.com) captures errors in real-time from your web,
mobile and desktop applications, helping you to understand and resolve them
as fast as possible. [Create a free account](https://www.bugsnag.com) to start
capturing exceptions from your applications.

Contents
--------

- [Getting Started](#getting-started)
	- [Installation](#installation)
	- [Sending a Test Notification](#sending-a-test-notification)
- [Usage](#usage)
	- [Catching and Reporting Exceptions](#catching-and-reporting-exceptions)
	- [Sending Handled Exceptions](#sending-handled-exceptions)
	- [Callbacks](#callbacks)
- [Demo Applications](#demo-applications)
- [Support](#support)
- [Contributing](#contributing)
- [License](#license)

- [Additional Documentation](https://docs.bugsnag.com/platforms/dotnet/)
	- [Configuration](https://docs.bugsnag.com/platforms/dotnet/other/configuration-options/)



Getting Started
---------------

### Installation

#### Using Nuget (Recommended)
- Install the [Bugsnag](https://www.nuget.org/packages/Bugsnag/) package from Nuget.

#### Manual library reference
- Download the latest [Bugsnag.dll](https://github.com/bugsnag/bugsnag-dotnet/releases/latest) and reference it in your project

### Sending a Test Notification

```
var configuration = new Bugsnag.Configuration("{API_KEY}");
var client = new Bugsnag.Client(configuration);

client.Notify(new System.Exception("Error!"));
```

Usage
-----

### Catching and Reporting Exceptions

```
var configuration = new Bugsnag.Configuration("{API_KEY}");
var client = new Bugsnag.Client(configuration);

throw new System.Exception("Error!");
```

### Sending Handled Exceptions

```
var configuration = new Bugsnag.Configuration("{API_KEY}");
var client = new Bugsnag.Client(configuration);

try
{
	throw new System.Exception("Error!");
}
catch (System.Exception ex)
{
	client.Notify(ex);
}
```

### Callbacks

```
var configuration = new Bugsnag.Configuration("{API_KEY}");
var client = new Bugsnag.Client(configuration);
client.BeforeNotify((configuration, report) => {
	report.User = new Bugsnag.Payload.User() { Name = "Testy McTest" };
});
```

### Disabling Bugsnag in Debug Mode

```
var configuration = new Bugsnag.Configuration("{API_KEY}")
{
	ReleaseStage = "development",
	NotifyReleaseStages = new[] { "production" },
};
var client = new Bugsnag.Client(configuration);

try
{
	throw new System.Exception("Error!");
}
catch (System.Exception ex)
{
	client.Notify(ex);
}
```

Demo Applications
-----------------

[Demo applications which use the Bugsnag .NET library](examples/)

Support
-------

* [Additional Documentation](https://docs.bugsnag.com/platforms/dotnet/)
* [Search open and closed issues](https://github.com/bugsnag/bugsnag-dotnet/issues?utf8=✓&q=is%3Aissue) for similar problems
* [Report a bug or request a feature](https://github.com/bugsnag/bugsnag-dotnet/issues/new)

Contributing
------------

We'd love you to file issues and send pull requests. The [contributing guidelines](CONTRIBUTING.md) details the process of building and testing `bugsnag-dotnet`, as well as the pull request process. Feel free to comment on [existing issues](https://github.com/bugsnag/bugsnag-dotnet/issues) for clarification or starting points.

License
-------

The Bugsnag .NET notifier is free software released under the MIT License.
