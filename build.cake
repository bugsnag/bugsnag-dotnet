#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");
// this can be changed once we are pushing the nuget packages up
var nugetPackageOutput = MakeAbsolute(Directory("./packages"));
var configuration = Argument("configuration", "Release");
var examples = GetSubDirectories("./examples");

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");
    CleanDirectories("./**/packages");
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
    XUnit2(GetFiles($"./tests/**/bin/{configuration}/**/*.Tests.dll"));
  });

Task("Pack")
  .IsDependentOn("Test")
  .Does(() =>
{
  MSBuild("./Bugsnag.sln", settings =>
    settings
      .SetVerbosity(Verbosity.Minimal)
      .WithTarget("pack")
      .SetConfiguration(configuration)
      .WithProperty("IncludeSymbols", "true")
      .WithProperty("PackageOutputPath", nugetPackageOutput.FullPath));
});

Task("PopulateExamplePackages")
  .IsDependentOn("Pack")
  .Does(() =>
{
      foreach (var directory in examples)
      {
          CopyDirectory(nugetPackageOutput, directory.Combine(new DirectoryPath("packages")));
      }
});

Task("BuildExamples")
  .IsDependentOn("PopulateExamplePackages")
  .Does(() =>
{
      var failures = examples.AsParallel().Select(e => {
        IEnumerable<string> stdOut;
        IEnumerable<string> errOut;
        var settings = new ProcessSettings { Arguments = "build", WorkingDirectory = e, RedirectStandardOutput = true, RedirectStandardError = true };
        var exitCode = StartProcess("docker-compose", settings, out stdOut, out errOut);
        Information("docker-compose build {0}", e);
        return new { ExitCode = exitCode, StdOutput = stdOut, ErrOutput = errOut, Example = e };
      }).Where(o => o.ExitCode != 0).ToArray();

      foreach (var failure in failures)
      {
        Error(failure.Example);
        foreach (var output in failure.StdOutput)
        {
          Error(output);
        }
        foreach (var output in failure.ErrOutput)
        {
          Error(output);
        }
      }

      if (failures.Any())
      {
        throw new Exception("Failed to build examples");
      }
});

Task("Default")
    .IsDependentOn("BuildExamples");

RunTarget(target);
