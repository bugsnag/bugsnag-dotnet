
var solution = File("./Bugsnag.sln");
var target = Argument("target", "Default");
var buildDir = Directory("./build");
var nugetPackageOutput = buildDir + Directory("packages");
var configuration = Argument("configuration", "Release");
var examples = GetFiles("./examples/**/*.sln");
var buildProps = File("./src/Directory.build.props");
string version = "1.0.0";

Task("Clean")
    .Does(() => CleanDirectory(buildDir));

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() => NuGetRestore(solution));

Task("Build")
  .IsDependentOn("Restore-NuGet-Packages")
  .Does(() => {
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
      var settings = new DotNetTestSettings
      {
        Configuration = configuration,
        ArgumentCustomization = args => {
          if (AppVeyor.IsRunningOnAppVeyor)
          {
            args.Append("-appveyor");
          }
          return args;
        }
      };

      foreach(var file in testAssemblies)
      {
        DotNetTest(file.FullPath, "MSTest.MapInconclusiveToFailed=true", settings);
      }
    });

Task("Pack")
  .IsDependentOn("Test")
  .Does(() => {
    MSBuild(solution, settings =>
      settings
        .SetVerbosity(Verbosity.Minimal)
        .WithTarget("pack")
        .SetConfiguration(configuration)
        .WithProperty("IncludeSymbols", "true")
        .WithProperty("SymbolPackageFormat", "snupkg")
        .WithProperty("GenerateDocumentationFile", "true")
        .WithProperty("PackageOutputPath", MakeAbsolute(nugetPackageOutput).FullPath));
  });

Task("BuildExamples")
  .Does(() => {
    foreach (var example in examples) {
      NuGetRestore(example);
      MSBuild(example, settings =>
        settings
          .SetVerbosity(Verbosity.Minimal));
    }
  });

Task("SetVersion")
  .Does(() => {
    version = AppVeyor.Environment.Build.Version;
    if (AppVeyor.Environment.Repository.Tag.IsTag)
    {
      version = AppVeyor.Environment.Repository.Tag.Name.TrimStart('v');
    }
    else
    {
      version = $"{version}-dev-{AppVeyor.Environment.Repository.Commit.Id.Substring(0, 7)}";
    }
    AppVeyor.UpdateBuildVersion(version);
    var path = "/Project/PropertyGroup/Version";
    XmlPoke(buildProps,  path, version);
  });


Task("MazeRunner")
  .IsDependentOn("Pack")
  .Does(() => {
    StartProcess("cmd", "/c bundle install");
    var mazeRunner = StartProcess("cmd", $"/c \"set BUGSNAG_VERSION={version} && bundle exec bugsnag-maze-runner --verbose\"");
    if (mazeRunner != 0) {
      throw new Exception("maze-runner failed");
    }
  });

Task("Default")
  .IsDependentOn("Test");

Task("Appveyor")
  .IsDependentOn("SetVersion")
  .IsDependentOn("Pack")
  .IsDependentOn("MazeRunner");

RunTarget(target);
