Upgrading
=========

## 1.x to 2.x

*Our .NET notifier has gone through some major improvements, and there are some changes you'll need to make to get onto the new version.*

##### MVC applications

We now have a dedicated Nuget package for ASP.NET MVC applications. In order to make sure that you have the new version you should uninstall the [Bugsnag](https://www.nuget.org/packages/Bugsnag/) Nuget package and install the [Bugsnag.AspNet.Mvc](https://www.nuget.org/packages/Bugsnag.AspNet.Mvc/) package.

The Bugsnag config section in your `Web.config` will need to be changed slightly.

```diff
 <configuration>
   <configSections>
-    <section name="bugsnagConfig" type="Bugsnag.ConfigurationStorage.ConfigSection, Bugsnag" />
+    <section name="bugsnag" type="Bugsnag.ConfigurationSection.Configuration, Bugsnag.ConfigurationSection" />
   </configSections>
-  <bugsnagConfig apiKey="your-api-key-goes-here" />
+  <bugsnag apiKey="your-api-key-goes-here" />
 </configuration>
```

Next you will need to remove the Bugsnag global filter, this is now included automatically if you are targeting at least .NET 4.0.

```diff
- filters.Add(WebMVCClient.ErrorHandler());
```

If you are targeting .NET 3.5 please follow the directions provided in the [documentation](https://docs.bugsnag.com/platforms/dotnet/mvc/#basic-configuration) to complete the upgrade.

##### Web API applications

We now have a dedicated Nuget package for ASP.NET Web API applications. In order to make sure that you have the new version you should uninstall the [Bugsnag](https://www.nuget.org/packages/Bugsnag/) Nuget package and install the [Bugsnag.AspNet.WebApi](https://www.nuget.org/packages/Bugsnag.AspNet.WebApi/) package.

The Bugsnag config section in your `Web.config` will need to be changed slightly.

```diff
 <configuration>
   <configSections>
-    <section name="bugsnagConfig" type="Bugsnag.ConfigurationStorage.ConfigSection, Bugsnag" />
+    <section name="bugsnag" type="Bugsnag.ConfigurationSection.Configuration, Bugsnag.ConfigurationSection" />
   </configSections>
-  <bugsnagConfig apiKey="your-api-key-goes-here" />
+  <bugsnag apiKey="your-api-key-goes-here" />
 </configuration>
```

Next you will need to remove the Bugsnag global filter, this is now included automatically if you are targeting at least .NET 4.0.

```diff
- filters.Add(WebAPIClient.ErrorHandler());
```

If you are targeting .NET 3.5 please follow the directions provided in the [documentation](https://docs.bugsnag.com/platforms/dotnet/web-api/#basic-configuration) to complete the upgrade.

##### WPF applications

The latest supported `bugsnag-dotnet` library for WPF applications is [v1.4.0](https://github.com/bugsnag/bugsnag-dotnet/releases/tag/v1.4.0). Please refer to the [Bugsnag WPF application docs](https://docs.bugsnag.com/platforms/dotnet/wpf/) for more details on how to install and configure Bugsnag in your WPF application.

##### Other applications

[Update](https://docs.microsoft.com/en-us/nuget/consume-packages/reinstalling-and-updating-packages) your [Bugsnag](https://www.nuget.org/packages/Bugsnag/) Nuget package to version 2.0.0. If you are using an `App.config` to configure Bugsnag then you will also need to add the [Bugsnag.ConfigurationSection](https://www.nuget.org/packages/Bugsnag.ConfigurationSection/) package.

The Bugsnag config section in your `App.config` will need to be changed slightly.

```diff
 <configuration>
   <configSections>
-    <section name="bugsnagConfig" type="Bugsnag.ConfigurationStorage.ConfigSection, Bugsnag" />
+    <section name="bugsnag" type="Bugsnag.ConfigurationSection.Configuration, Bugsnag.ConfigurationSection" />
   </configSections>
-  <bugsnagConfig apiKey="your-api-key-goes-here" />
+  <bugsnag apiKey="your-api-key-goes-here" />
 </configuration>
```

You will then want to construct an instance of the Bugsnag client to use within your application.

```diff
+ var client = new Bugsnag.Client(Bugsnag.ConfigurationSection.Configuration.Settings);
```
