var target = Argument("target", "Default");
var nugetPackageOutput = MakeAbsolute(Directory("./examples/aspnetcore20-mvc/packages"));
var configuration = Argument("configuration", "Release");
var buildDir = Directory("./src/Example/bin") + Directory(configuration);

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
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
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./Bugsnag.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./Bugsnag.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Pack")
  .IsDependentOn("Build")
  .Does(() =>
{
    MSBuild("./src/Bugsnag/Bugsnag.csproj", settings =>
      settings
        .WithTarget("pack")
        .WithProperty("PackageOutputPath", nugetPackageOutput.FullPath));
    MSBuild("./src/Bugsnag.AspNet.Core/Bugsnag.AspNet.Core.csproj", settings =>
      settings
        .WithTarget("pack")
        .WithProperty("PackageOutputPath", nugetPackageOutput.FullPath));
});

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);
