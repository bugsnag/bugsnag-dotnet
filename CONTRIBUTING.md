# Contribution Guide

## Development Dependencies

- Windows 10 pro
- Docker for Windows
- Visual Studio 2017
- [.NET framework 3.5](https://docs.microsoft.com/en-us/dotnet/framework/install/dotnet-35-windows-10)

## Running the build script

Set Powershell execution policy to RemoteSigned. You can check this by running

`Get-ExecutionPolicy`

This should return

`RemoteSigned`

If not you will need to launch Powershell with "Run as administrator" option and then run

`Set-ExecutionPolicy RemoteSigned`

## Building the Project

`.\build.ps1 -Target Build`

## Running the Tests

`.\build.ps1 -Target Test`

## Building/Running Example Apps

See the [README](examples)

## Submitting a Change

* [Fork](https://help.github.com/articles/fork-a-repo) the
  [notifier on github](https://github.com/bugsnag/bugsnag-dotnet)
* Commit and push until you are happy with your contribution
* Run the tests and ensure they all pass
* [Submit a pull request](https://help.github.com/articles/using-pull-requests)
* Thank you!

## Useful References

----

## Release Guidelines

If you're a member of the core team, follow these instructions for releasing
bugsnag-dotnet.

### First-time setup

### Every time

* Compile new features, enhancements, and fixes into the CHANGELOG.
* Update the project version using [semantic versioning](http://semver.org).
  Specifically:

  > Given a version number MAJOR.MINOR.PATCH, increment the:
  >
  > 1. MAJOR version when you make incompatible API changes,
  > 2. MINOR version when you add functionality in a backwards-compatible
  >    manner, and
  > 3. PATCH version when you make backwards-compatible bug fixes.
  >
  > Additional labels for pre-release and build metadata are available as
  > extensions to the MAJOR.MINOR.PATCH format.

* Commit and push your changes
* Add a git tag with the new version of the library
