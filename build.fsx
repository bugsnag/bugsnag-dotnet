#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing
open Fake.Git

let solution = "Bugsnag.sln"
let version = "1.4.0.0"
let buildConfigs = ["Release"; "MonoRelease"]
let tests = !! "test/bin/**/Bugsnag.Test.dll"
let buildDir = "./build/"
let build props = MSBuild "" "Build" props [solution] |> ignore
let nuget spec = NuGet (fun p ->
                        {p with
                          Version = version
                          OutputPath = buildDir
                          WorkingDir = "." })
                          spec

Target "RestorePackages" (fun _ ->
    solution
    |> RestoreMSSolutionPackages (fun p ->
         { p with
             Retries = 4 })
)

Target "Clean" (fun _ ->
  showGitCommand "." "clean -xdf src"
  CleanDir buildDir
)

Target "Build" (fun _ ->
  buildConfigs
    |> List.map (fun x -> ["Configuration", x])
    |> List.iter build
)

Target "Test" (fun _ ->
  tests
    |> xUnit (fun p -> {p with ShadowCopy = false})
)

Target "Package" (fun _ ->
  !! "*.nuspec"
    |> Seq.iter nuget

  !! "src/Bugsnag/bin/MonoRelease/**/*"
    |> Seq.iter (fun file -> CopyFileWithSubfolder "src/Bugsnag/bin/MonoRelease" buildDir file)

  let buildDirInfo = directoryInfo buildDir
  let monoRename (d : System.IO.DirectoryInfo) = Rename (buildDir + "mono-" + d.Name) (buildDir + d.Name)

  subDirectories buildDirInfo
    |> Seq.iter monoRename

  !! "src/Bugsnag/bin/Release/**/*"
    |> Seq.iter (fun file -> CopyFileWithSubfolder "src/Bugsnag/bin/Release" buildDir file)
)

"Clean"
  ==> "RestorePackages"
  ==> "Build"
  ==> "Test"
  ==> "Package"

RunTargetOrDefault "Package"
