#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");

// this can be changed once we are pushing the nuget packages up
var nugetPackageOutput = MakeAbsolute(Directory("./examples/aspnetcore20-mvc/packages"));

var configuration = Argument("configuration", "Release");

var tests = new[] {"Bugsnag.Tests", "Bugsnag.AspNet.Tests"};
var projects = new[] {"Bugsnag", "Bugsnag.AspNet", "Bugsnag.AspNet.Core", "Bugsnag.AspNet.Mvc", "Bugsnag.ConfigurationSection"};

Task("Clean")
    .Does(() =>
{
    CleanDirectory(nugetPackageOutput);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./Bugsnag.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
  MSBuild("./Bugsnag.sln", settings =>
    settings
      .SetVerbosity(Verbosity.Minimal)
      .SetConfiguration(configuration));
});

Task("Test")
  .IsDependentOn("Build")
  .Does(() => {
    foreach (var test in tests)
    {
      XUnit2($"./tests/{test}/bin/{configuration}/net461/win10-x64/{test}.dll");
    }
    });

Task("Pack")
  .IsDependentOn("Test")
  .Does(() =>
{
    foreach(var project in projects)
    {
      MSBuild($"./src/{project}/{project}.csproj", settings =>
        settings
          .SetVerbosity(Verbosity.Minimal)
          .WithTarget("pack")
          .WithProperty("PackageOutputPath", nugetPackageOutput.FullPath));
    }
});

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);
