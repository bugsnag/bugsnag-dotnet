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
Note that the API key used by the client can be examined at any time
```c#
string apiKey = bugsnag.Config.ApiKey;
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
metadata.AddToTab("Resources", "Datastore Entries", myDataStore.Count());
metadata.AddToTab("Resources", "Threads Running", threads.Count());
metadata.AddToTab("Resources", "Throttling Enabled", false);

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

Additional Configuration
------------------------
#### Application Settings
The client can be configured with specific application settings which can be used to determine when and what information to send. Application settings can be set at any time, but are normally set after the client is created.

##### Application Version
Used to attach the version number of the application which generated the error. If the application version is set and an error is resolved in the dashboard the error will not be unresolved until a crash is seen in a newer version of the app.
```c#
bugsnag.Config.AppVersion = "5.0.1";
```

##### Release Stage
Represents the current release stage for the application. This needs to be set manually.
```c#
bugsnag.Config.ReleaseStage = "staging";
```
By default, the client will report on all errors regardless of the release stage. However, you can filter out errors from certain stages. To specify which stages you want to notify on, use the following method.
```c#
bugsnag.Config.SetNotifyReleaseStages("development","production");
```
If you want to switch back to reporting on all release stages call this method.
```c#
bugsnag.Config.NotifyOnAllReleaseStages();
```

#####  Endpoint URL
All notifications are send to the Endpoint URL. By default, this is set to `notify.bugsnag.com`, however this can be overridden.
```c#
bugsnag.Config.Endpoint = "myserver.bugsnag.com";
```
Notification are sent using SSL by default. However, this can be disabled
```c#
bugsnag.Config.UseSsl = false;
```

#### Additional Notification Data
The client can be configured to provide additional information based on the current error or process that is running at the time. These settings can be set any time.

##### Context
The concept of "contexts" is used to help display and group your errors. Contexts represent what was happening in your application at the time an error occurs.
```c#
bugsnag.Config.Context = "DataAccess";
```

##### User Details
The client can be configured to send the users ID, email and name with every notification. The user ID should be set to something unique to the user e.g. a generated GUID.
```c#
bugsnag.Config.SetUser("d7b4aadd", "anth.michaels@mycompany.com", "Anthony Michaels");
```

##### Client Metadata
Metadata can be set directly on the client. This metadata will be sent with every error report (in addition to any metadata added to the report itself).
```c#
bugsnag.Config.Metadata.AddToTab("Company", "Department", "Human Resources");
bugsnag.Config.Metadata.AddToTab("Company", "Location", "New York");
```

#### Notification Settings
The client can be configured to restrict or modify the notifications that are sent to Bugsnag. These settings will allow you suppress specific errors, remove unnecessary information and modify the notification before its sent.

##### File Prefixes
When an exception stack trace is recorded, the file associated with each frame will be recorded, if its available. This will be the complete file path, which can lead to bloated frame entries. The paths will also reflect where the application was complied. The client can be configure to remove common file path prefixes.
```c#
bugsnag.Config.SetFilePrefix(@"C:\Projects\Production\MyApp\",
                             @"C:\Projects\Development\MyApp\",
                             @"H:\MyApp\");
```
##### Project Namespaces
Bugsnag will highlight stack trace frames if they are detected as being *In Project*. The client can be configured with project namespaces. If an stack trace frame method call originates from a class that belongs to one of these project namespaces, they will be highlighted.
```c#
bugsnag.Config.SetFilePrefix("MyCompany.MyApp","MyCompany.MyLibrary");
```
##### Auto Detect In Project
Debugging information is used to provide file paths for stack frames. Normally, this information is only available for locally built projects. Therefore, in most cases, stack frames that have file information relate to calls made within the users code. We use this fact to automatically mark these frames as *In Project* by default. This is in addition to any project namespaces that have been manually added. This behaviour can be disabled.
```c#
// Disable marking frames with file names as In Project
bugsnag.Config.AutoDetectInProject = false;
```
If no project namespaces have been configured and auto detect has been disabled, all stack frames will be highlighted.

##### Ignore Exception Classes
The client can be configured to ignore specific types of exceptions. Any errors with these types of exceptions will not be sent to Bugsnag.
```c#
bugsnag.Config.SetIgnoreClasses("ArgumentNullException", "MyConfigException");
```

##### Before Notify Callback
A custom call back function can be configured to run just before an error event is sent. The callback has full access to the error and can modify it before its sent. It also has the opportunity to prevent the error from being sent all together. The callback should take an error `Event` object as a parameter and return a boolean indicating if the event should be notified (`Func<Event,bool>`);

Note that the callback will not be called if the exception class is an class being ignored via `SetIgnoreClasses()`.

```c#
bugsnag.Config.BeforeNotifyCallback = error =>
{
    // Sets the groupingHash option
    error.setGroupingHash("My Group");

    // Elevate all warnings as errors
    if (error.Severity == Severity.Warning)
    {
        error.Severity = Severity.Error;
        error.addToTab("Reporting", "Elevated", true);
    }

    // Ignore all exceptions marked as minor
    var isMinor = error.Exception.Message.Contains("[MINOR]");
    return !isMinor;
};
```
