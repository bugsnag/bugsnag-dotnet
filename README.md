Bugsnag Notifier for .NET
===========

[![Build status](https://ci.appveyor.com/api/projects/status/mbeihwth6o30h329?svg=true)](https://ci.appveyor.com/project/CodeHex/bugsnag-net)

The Bugsnag Notifier for .NET gives you instant notification of exceptions thrown from your .NET applications. The notifier hooks into various handlers so any uncaught exceptions in your app will be sent to your Bugsnag dashboard.

- The majority of exceptions are handled by hooking into the `AppDomain.CurrentDomain.UnhandledException` event.
- Exceptions that occur in a `Task` are silently discarded if the are unobserved. We hook into the `TaskScheduler.UnobservedTaskException` so they can be reported ([Task Exception Handling in .NET 4.5](http://blogs.msdn.com/b/pfxteam/archive/2011/09/28/task-exception-handling-in-net-4-5.aspx)).
- The handler uses the `[HandleProcessCorruptedStateExceptions]` attribute to enable reporting on corrupted state exceptions such as memory access violation exceptions ([CorruptedStateExceptions in .NET](http://dailydotnettips.com/2013/09/23/corruptedstateexceptions-in-net-a-way-to-handle/)) .
- An exception filter `NotifyExceptionAttribute` to use as a global filter is provided for handling exceptions in ASP.NET MVC applications.

[Bugsnag](https://bugsnag.com) captures errors in real-time from your websites and mobile applications, helping you to understand and resolve them as fast as possible. [Create a free account](https://bugsnag.com) to start capturing errors from your applications.

How to Install
--------------

### Using Nuget (Recommended)
    TODO

### Manual Library Reference

- Download the latest Bugsnag.Core dll and reference it in your project (TODO add download location/link)
- For ASP.NET MVC applications, also download the latest Bugsnag.Web dll and reference it in your project to
access the global filter (TODO add download location/link).

Bugsnag for .NET depends only on the `JSON.net` library and needs to be referenced, the nuget package can be found [here](https://www.nuget.org/packages/Newtonsoft.Json/).

Quick Reference Guide
---------------------

Import the Bugnsnag core library into your application 

```c#
using Bugsnag.Core;
```

Create and instance of the client using your api key.
```c#
var bugsnag = new Client("your-api-key-goes-here");
```

Thats it...you will be catching and reporting on uncaught exceptions.







