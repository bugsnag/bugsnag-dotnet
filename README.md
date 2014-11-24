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
Create an instance of the client using your API key.
```c#
var bugsnag = new Client("your-api-key-goes-here");
```
Thats it...you will be reporting on uncaught exceptions by default.

Creating the client
---------------------
The client needs to be created using your API key. If you would prefer to disable the auto notification, you can create the client with auto notify turned off (on by default).
```c#
var bugsnag = new Client("your-api-key-goes-here", false);
```
To manually start or stop auto notification at any point...
```c#
// To stop auto notification
bugsnag.StopAutoNotify();

// To start auto notification
bugsnag.StartAutoNotify();
```

Send Non-Fatal Exceptions to Bugsnag
------------------------------------

If you would like to send non-fatal exceptions to Bugsnag, you can pass any
object that inherits from `Exception` to the `Notify` method:

```c#
bugsnag.Notify(new ArgumentException("Non-fatal"));
```
You can also send additional meta-data with your exception:
```c#
var metadata = new Metadata();
metadata.AddToTab("Company", "Department", "Human Resources");
metadata.AddToTab("Company", "Location", "New York");

bugsnag.Notify(new ArgumentException("Non-fatal"), metadata);
```
You can set the severity of an error in Bugsnag by including the severity option when
notifying bugsnag of the error,
```c#
bugsnag.Notify(new ArgumentException("Non-fatal"), Severity.Info)
```
Valid severities are `Severity.Error`, `Severity.Warning` and `Severity.Info`.

Severity is displayed in the dashboard and can be used to filter the error list.
By default all crashes (or unhandled exceptions) are set to `Severity.Error` and all
`bugsnag.Notify()` calls default to `Severity.Warning`.


