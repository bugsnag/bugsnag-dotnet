Changelog
=========

## Unreleased

### Bug fixes

* Handle any exceptions raised when reading files for code segments 
  | [twometresteve](https://github.com/twometresteve)
  | [#123](https://github.com/bugsnag/bugsnag-dotnet/pull/123)

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
