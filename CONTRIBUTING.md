Building Bugsnag
----------------

Bugsnag can be built either using AppVeyor.

Build using AppVeyor
--------------------

 * Commit the change. This should kick off a build on AppVeyor (if configured correctly). Otherwise you can manually kick off a build via the website.

Releasing
---------

* Update the CHANGELOG.md
* Update the version of the build in `appveyor.yml`, located in the root directory.
* Update the version of the build in `build.fsx`, located in the root directory.
* Update the version of the build in `Bugsnag.nuspec` and `BugsnagMono.nuspec`, located in the root directory.
* Update the version of the build in `CommonVersionInfo.cs`, located in the `src/Common`.
* Push the change, this should kick off a build.
* Tag master with the correct version `git tag v1.2.0; git push --tags`
* Download the zip file containing the binaries in the **ARTIFACTS** section.
* Upload the packages to nuget (extract from the zipfile)
* Upload the packages to github and perform a github release
* Update the setup guides for .NET (and its frameworks) on docs.bugsnag.com
  with any new content.
