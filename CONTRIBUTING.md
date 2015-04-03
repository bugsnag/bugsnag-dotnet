Building Bugsnag
----------------

Bugsnag can be built either using a local Windows build environment or by using AppVeyor.

Build using Local Windows Environment
--------------------

### Prerequisites

Use [chocolately](https://chocolatey.org/) to install the following build tools

```
choco install psake
choco install pscx
choco install nuget.commandline
```

### How to Build

1. Download Git repo of Bugsnag
2. Edit file `build\build.ps1` and set the version of the build (near the top of the file)
```ps
$default_version = "1.2.3.4"
```
3. Execute `build\build.bat`

### Build Output

The build will produce the following structure in the `build` folder

```
+-- output
    +-- bin
    +-- nuget
    +-- test
    --- bugsnag.{version}.zip
    --- Bugsnag.{version}.nupkg
    --- BugsnagMono.{version}.nupkg
```

 * `bin` folder - Contains all binaries for specified configurations
 * `nuget` folder - Contains the files used to create the nuget packages
 * `test` folder - Contains all binaries used when testing the build
 * `bugsnag.{version}.zip` - Archive of binaries (basically just the `bin` folder zipped up)
 * `Bugsnag.{version}.nupkg` - Nuget package for Bugsnag
 * `BugsnagMono.{version}.nupkg` - Nuget package for Mono compatible version of Bugsnag


### Clean Previous Build
Run `build\cleanbuild.bat` to remove the previous builds output

Build using AppVeyor
--------------------

 * Commit the change. This should kick off a build on AppVeyor (if configured correctly). Otherwise you can manually kick off a build via the website.

Releasing 
---------

* Update the CHANGELOG.md
* Update the version of the build in `appveyor.yml`, located in the root directory.
* Push the change, this should kick off a build.
* Tag master with the correct version `git tag v1.1.0; git push --tags`
* Download the nuget packages and zip file containing the binaries in the **ARTIFACTS** section.
* Upload the packages to nuget
* Upload the packages to github and perform a github release
