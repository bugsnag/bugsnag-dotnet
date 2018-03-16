#tool "nuget:?package=xunit.runner.console"
#addin nuget:?package=Cake.SemVer
#addin nuget:?package=semver&version=2.0.4

var solution = File("./Bugsnag.sln");
var target = Argument("target", "Default");
var buildDir = Directory("./build");
var nugetPackageOutput = buildDir + Directory("packages");
var configuration = Argument("configuration", "Release");
var examples = GetSubDirectories("./examples");
var buildProps = File("./src/Directory.build.props");

Task("Clean")
    .Does(() => CleanDirectory(buildDir));

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() => NuGetRestore(solution));

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
  MSBuild(solution, settings =>
    settings
      .WithProperty("BaseOutputPath", $"{MakeAbsolute(buildDir).FullPath}\\")
      .SetVerbosity(Verbosity.Minimal)
      .SetConfiguration(configuration));
});

Task("Test")
  .IsDependentOn("Build")
  .Does(() => {
    var testAssemblies = GetFiles($"{buildDir}/**/*.Tests.dll");
    XUnit2(testAssemblies,
      new XUnit2Settings {
          ArgumentCustomization = args => {
            if (AppVeyor.IsRunningOnAppVeyor)
            {
              args.Append("-appveyor");
            }
            return args;
          }
      });
  });

Task("Pack")
  .IsDependentOn("Test")
  .Does(() =>
{
  MSBuild(solution, settings =>
    settings
      .SetVerbosity(Verbosity.Minimal)
      .WithTarget("pack")
      .SetConfiguration(configuration)
      .WithProperty("IncludeSymbols", "true")
      .WithProperty("GenerateDocumentationFile", "true")
      .WithProperty("PackageOutputPath", MakeAbsolute(nugetPackageOutput).FullPath));
});

Task("BuildExamples")
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

Task("PokeBuildNumber")
  .Does(() =>
{
    var path = "/Project/PropertyGroup/Version";
    XmlPoke(
      buildProps,
      path,
      ParseSemVer(XmlPeek(buildProps, path)).Change(patch: AppVeyor.Environment.Build.Number).ToString());
});

Task("Default")
    .IsDependentOn("Test");

Task("Appveyor")
    .IsDependentOn("PokeBuildNumber")
    .IsDependentOn("Pack");

RunTarget(target);
