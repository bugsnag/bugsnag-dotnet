Bugsnag Notifier for .NET
===========

[![Build status](https://ci.appveyor.com/api/projects/status/mbeihwth6o30h329/branch/master?svg=true)](https://ci.appveyor.com/project/CodeHex/bugsnag-net)

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

Adding Bugsnag to an ASP.NET MVC project
-------------------------------------------
The attribute `NotifyExceptionAttribute` found in the `Bugsnag.Web` assembly needs to
be registered as a global filter. Normally, this can be done in the `App_Start > FilterConfig.cs`
file.
```c#
using Bugsnag.Web;
using System.Web.Mvc;

namespace Web.MVCApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Register the notify attribute
            filters.Add(new NotifyExceptionAttribute("your-api-key-goes-here"));

            filters.Add(new HandleErrorAttribute());
        }
    }
}

```
To configure the client, use the client property available on the attribute
```c#
var bugsnagAttrib = new NotifyExceptionAttribute("your-api-key-goes-here");

// Access the client on the attribute to configure e.g.
bugsnagAttrib.Client.Config.AppVersion = "0.1.2";
```

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
Debugging information is used to provide file paths for stack frames. Normally, this information is only available for locally built projects. Therefore, in most cases, stack frames that have file information relate to calls made within the users code. We use this fact to automatically mark these frames as *In Project* by default. This is in addition to any project namespaces that have been manually added. This behavior can be disabled.
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

##### Before Notify Callbacks
Custom call back functiosn can be configured to run just before an error event is sent. These callbacks have full access to the error and can modify it before its sent. They also have the opportunity to prevent the error from being sent all together. A callback should take an error `Event` object as a parameter and return a boolean indicating if the event should be notified (`Func<Event,bool>`);

Note that these callbacks will not be called if the exception class is an class being ignored via `SetIgnoreClasses()` or the current release stage is one that has been configured not to be notified on via `SetNotifyReleaseStages()`.

```c#
bugsnag.Config.BeforeNotify(error =>
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
});
```

##### Filters
Data associated with notifications are sent via the `Metadata` object attached to the error. Sensitive information can be filtered before its sent to Bugsnag by setting filters. Any tab entries that have keys matching these filters will have they value replaced with the text `[FILTERED]`
```c#
bugsnag.Config.SetFilters("Password", "Credit Card Number");
```
Error Event Object
------------------
When an exception occurs, the details of the exception is collated and recorded into an error `Event` object. This object stores all the information about the exception that will be sent to Bugsnag. By setting this object properties, you can send additional information or modify an existing object before its sent.

These error event objects can be created directly and sent to Bugsnag.
```c#
var error = new Event(new StackOverflowException("Too many stack levels"));
bugsnag.Notify(error);
```
Note that if you create an exception without it actually being throw, as in the above example, there will be no stack trace. If this occurs, we will record and report the Call Stack Trace i.e. the stack trace relating to when the `Event` object was created.

All event objects can be modified just before they are sent to Bugsnag using the [Before Notify Callback](#Before Notify Callback)

The available properties available

#### Exception (Read Only)
The exception the event is representing. This needs to be provided when creating an error event and can not be modified.
```c#
Exception exp = error.Exception;
```

#### IsRuntimeEnding (Read Only)
If an exception occurs that has resulted in the application crashing out (runtime ended), this flag will be true.
```c#
bool hasCrashed = error.IsRuntimeEnding;
```

#### CallTrace (Read Only)
If the exception stack trace is unavailable a call stack trace will be recorded at the point the `Event` object was created. For efficiency, the call stack trace will be `null` if the exception stack trace was found.
```c#
StackTrace callTrace = error.CallTrace;
```

#### GroupingHash
Sets the grouping hash used by Bugsnag.com to manually override the default grouping technique. This option is not recommended, and should only be manually set with care.

Any errors that are sent to Bugsnag, that have the same grouping hash will be grouped as one.
```c#
error.GroupingHash = "d41d8cd98f00b204e9800998ecf8427e";
```

#### Severity
The severity can be set directly on the event. Valid severities are `Severity.Error`, `Severity.Warning` and `Severity.Info`.
```c#
error.Severity = Severity.Info;
```

#### Metadata
Additional information that you want to include with an error event is done using the Metadata property. The metadata represents tabs and tab entries that can be visualized from the Bugsnag dashboard. Each tab entry consists of a entry key and an entry value. The entry key is string, but the entry value can be any object that can serialized in JSON e.g. dictionaries, arrays, complex objects etc.

Each event object will start with a blank metadata object ready to be added to. However you can create your own metadata object and set the property directly. The metadata object has methods to help you add your own data

##### AddToTab
This is the main way to add data to a Metadata object. Specify the tab, tab entry key and tab entry value you want to add. If the entry key already exists in the tab your trying to add to, the value will be overwritten with the new value. If no tab is specified, the data will be added to the "Custom Data" tab.
```c#
// Adds company details under the "Company Details" tab
error.Metadata.AddToTab("Company Details", "Name", "My Company");
error.Metadata.AddToTab("Company Details", "Phone", "01123456789012");
error.Metadata.AddToTab("Company Details", "Email", "admin@mycompany.com");

// Adds a list of developers under a single entry
var devs = new string[]{"Bob Adams", "James Richards", "Lucy Patrick"};
error.Metadata.AddToTab("Team", "Developers", devs);

// Adds some app data to the custom data tab
error.Metadata.AddToTab("Full Test Suite", true);
```

##### RemoveTab
Removes a tab that exists in the metadata. Will do nothing if there is no such tab.
```c#
// Removes the Dev Only tab
error.Metadata.RemoveTab("Dev Only");
```

##### RemoveTabEntry
Removes a tab that exists in the metadata. Will do nothing if there is no such tab or tab entry.
```c#
// Removes the unit test result entry
error.Metadata.RemoveTabEntry("Build Results", "Test Results");
```
