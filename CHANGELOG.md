Changelog
=========

## 3.0.1 (2022-03-24)

### Bug fixes

* Ensure RemoveProjectRoots works for assemblies that are run on a OS that differs from their build OS.
  | [sgtfrankieboy](https://github.com/sgtfrankieboy)
  | [#154](https://github.com/bugsnag/bugsnag-dotnet/pull/154)

## 3.0.0 (2022-01-31)

### Breaking Changes
 
The `Bugnsag.dll`, `Bugsnag.AspNet.dll`, `Bugsnag.AspNet.Mvc.dll` and `Bugsnag.AspNet.WebApi.dll` assemblies are now strong-name signed. The strong name key file `Bugsnag.snk` has been added to the repository. `Bugsnag.AspNet.Core.dll` remains unchanged. See the upgrade guide for more details.

### Bug fixes

* Strong name sign assemblies
  | [yousif-bugsnag](https://github.com/yousif-bugsnag)
  | [#151](https://github.com/bugsnag/bugsnag-dotnet/pull/151)
  
* Ensure breadcrumbs are returned in the correct order
  | [yousif-bugsnag](https://github.com/yousif-bugsnag)
  | [#150](https://github.com/bugsnag/bugsnag-dotnet/pull/150)

## 2.2.3 (2021-09-06)

### Enhancements

* Set the delivery queue thread name.
  | [xPaw](https://github.com/xPaw)
  | [#139](https://github.com/bugsnag/bugsnag-dotnet/pull/139)

* Remove the 30 character limit for breadcrumb names.
  | [yousif-bugsnag](https://github.com/yousif-bugsnag)
  | [#141](https://github.com/bugsnag/bugsnag-dotnet/pull/141)

* Switch to snupkg package format and publish symbols to nuget.org.
  | [yousif-bugsnag](https://github.com/yousif-bugsnag)
  | [#143](https://github.com/bugsnag/bugsnag-dotnet/pull/143)

### Bug fixes

* Fix a crash when stackframe method is null.
  | [yousif-bugsnag](https://github.com/yousif-bugsnag)
  | [#140](https://github.com/bugsnag/bugsnag-dotnet/pull/140)

## 2.2.2 (2021-03-18)

### Bug fixes

* Stop sending code snippets in error reports.
  | [yousif-bugsnag](https://github.com/yousif-bugsnag)
  | [#134](https://github.com/bugsnag/bugsnag-dotnet/pull/134)

* Assume default behaviour for UnobservedTaskExceptions when App.config does not exist.
  | [yousif-bugsnag](https://github.com/yousif-bugsnag)
  | [#135](https://github.com/bugsnag/bugsnag-dotnet/pull/135)

## 2.2.1 (2020-05-14)

### Bug fixes

* Handle any exceptions raised when reading files for code segments.
  | [twometresteve](https://github.com/twometresteve)
  | [#123](https://github.com/bugsnag/bugsnag-dotnet/pull/123)

* Account for process termination behavior when handling UnobservedTaskExceptions.
  | [twometresteve](https://github.com/twometresteve)
  | [#125](https://github.com/bugsnag/bugsnag-dotnet/pull/125)

## 2.2.0 (2018-07-19)

### Enhancements

* Add additional method of specifying ignore classes to allow for providing fully qualified assembly names in config files.
  | [martin308](https://github.com/martin308)
  | [#109](https://github.com/bugsnag/bugsnag-dotnet/pull/109)

* Include Configuration package with WebApi
  | [martin308](https://github.com/martin308)
  | [#105](https://github.com/bugsnag/bugsnag-dotnet/pull/105)

## 2.1.0 (2018-05-30)

### Enhancements

* Improve metadata filtering
  | [martin308](https://github.com/martin308)
  | [#90](https://github.com/bugsnag/bugsnag-dotnet/pull/90)

### Bug fixes

* Only send session data when it exists
  | [martin308](https://github.com/martin308)
  | [#98](https://github.com/bugsnag/bugsnag-dotnet/pull/98)

* Request context is now available in custom middleware
  | [martin308](https://github.com/martin308)
  | [#91](https://github.com/bugsnag/bugsnag-dotnet/pull/91)

* Only set the context to the current request URL if the context has not already been set
  | [tremlab](https://github.com/tremlab)
  | [#89](https://github.com/bugsnag/bugsnag-dotnet/pull/89)

* Optimize serialization
  | [ShamsulAmry](https://github.com/ShamsulAmry)
  | [#101](https://github.com/bugsnag/bugsnag-dotnet/pull/101)

* Fix InProject namespaces detection
  | [jviolas](https://github.com/jviolas)
  | [#102](https://github.com/bugsnag/bugsnag-dotnet/pull/102)

* Improve handling of exceptions when sending data
  | [martin308](https://github.com/martin308)
  | [#103](https://github.com/bugsnag/bugsnag-dotnet/pull/103)

## 2.0.2 (2018-03-28)

### Bug fixes

* Allow the severity to be changed in callbacks
  | [martin308](https://github.com/martin308)
  | [#84](https://github.com/bugsnag/bugsnag-dotnet/pull/84)

## 2.0.1 (2018-03-27)

### Bug fixes

* Allow the background thread used to deliver reports to be terminated
  | [martin308](https://github.com/martin308)
  | [#81](https://github.com/bugsnag/bugsnag-dotnet/pull/81)

## 2.0.0 (2018-03-26)

This is a major release to make the library clearer and easier to use and expand the support to new platforms. For upgrading instructions, see [the upgrading guide](UPGRADING.md#1x-to-2x).

### Enhancements

* Rewritten to support .NET Core
* ASP.NET Core support added
* Improved ASP.NET support
* Much more

## 1.4.0

### Enhancements

* Track whether an exception was captured automatically

## 1.3.0

### Enhancements

* Support offline storage

## 1.2.0

### Enhancements

* Add async notify support

## 1.1.0

### Enhancements

* .NET 3.5 Support
* Azure Support
