#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");

// this can be changed once we are pushing the nuget packages up
var nugetPackageOutput = MakeAbsolute(Directory("./packages"));

var configuration = Argument("configuration", "Release");

var tests = new[] {"Bugsnag.Tests", "Bugsnag.AspNet.Tests"};
var projects = new[] {"Bugsnag", "Bugsnag.AspNet", "Bugsnag.AspNet.Core", "Bugsnag.AspNet.Mvc", "Bugsnag.ConfigurationSection"};

var examples = GetSubDirectories("./examples");

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
          .SetConfiguration("Release")
          .WithProperty("IncludeSymbols", "true")
          .WithProperty("PackageOutputPath", nugetPackageOutput.FullPath));
    }
});

Task("PopulateExamplePackages")
  .IsDependentOn("Pack")
  .Does(() =>
{
      var packages = new DirectoryPath("./packages");
      foreach (var directory in GetSubDirectories("./examples"))
      {
          CopyDirectory(packages, directory.Combine(new DirectoryPath("packages")));
      }
});

Task("BuildExamples")
  .IsDependentOn("PopulateExamplePackages")
  .Does(() =>
{
      foreach (var example in examples)
      {
        var docker = StartProcess("docker-compose", new ProcessSettings { Arguments = "build", WorkingDirectory = example });
        if (docker != 0)
        {
          throw new Exception("docker build failed");
        }
      }
});

Task("Default")
    .IsDependentOn("BuildExamples");

RunTarget(target);
