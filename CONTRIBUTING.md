# Contribution Guide

## Development Dependencies

- Windows 10 or later
- Docker for Windows
- Visual Studio 2022 or later
- .NET SDK 6.0+
- .NET SDK 8.0 (for running tests)
- .NET Framework 4.6.2 targeting pack

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

### Versioning

This project follows [semantic versioning](http://semver.org):

> Given a version number MAJOR.MINOR.PATCH, increment the:
>
> 1. MAJOR version when you make incompatible API changes,
> 2. MINOR version when you add functionality in a backwards-compatible
>    manner, and
> 3. PATCH version when you make backwards-compatible bug fixes.

### Release steps

1. Create a release branch from `next` (e.g. `release/vX.Y.Z`)
2. Compile new features, enhancements, and fixes into the `CHANGELOG.md`
3. Make a PR from your release branch into `master`
4. Once the PR has been approved, merge the release branch into `master`
5. Create a git tag on `master` with the new version (e.g. `vX.Y.Z`)
6. Create a new release on GitHub https://github.com/bugsnag/bugsnag-dotnet/releases/new
  - Use the tag vX.Y.Z as the name of the release
  - Copy the release notes from `CHANGELOG.md`
7. Crate a PR from `master` to `next` to keep the branches in sync

### How deployment works

Pushing a git tag triggers an automatic deployment via AppVeyor CI (configured
in `appveyor.yml`):

* The `Appveyor` Cake task runs, which sets the package version from the git
  tag and creates NuGet packages (`.nupkg` and `.snupkg` symbol packages)
* AppVeyor's deploy step detects the tag (`appveyor_repo_tag: true`) and
  pushes all NuGet and symbol packages to **nuget.org**
* No manual publish step is needed — tagging the commit is what triggers the
  release
