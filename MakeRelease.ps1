$baseDir = Resolve-Path "$PSScriptRoot"
$outDir = "$baseDir\out"
$msbuildExe = "C:\Program Files (x86)\MSBuild\14.0\bin\MsBuild.exe"
$nugetExe = "$baseDir\.nuget\NuGet.exe"

$configsToBuild = {"Release", "MonoRelease"}

if ($env:APPVEYOR) {
  $testExe = "xunit.console.clr4"
  $testFlags = '/appveyor'
} else {
  $testExe = "$baseDir\tools\xunit\xunit.console.clr4.exe"
  $testFlags = ''
}

git clean -xdf
& "$nugetExe" restore "$baseDir\Bugsnag.sln"

foreach-object $configsToBuild | % { & $msbuildExe "$baseDir/Bugsnag.sln" "/p:Configuration=$_" }

$filesToTest = ls -r "$baseDir\test\bin\**\Bugsnag.Test.dll"
echo $filesToTest | % { & $testExe "$_" $testFlags }

mkdir -Path $outDir

& "$nugetExe" pack $baseDir\Bugsnag.nuspec -OutputDirectory $outDir
& "$nugetExe" pack $baseDir\BugsnagMono.nuspec -OutputDirectory $outDir
mv "$baseDir\src\Bugsnag\bin\Release\*" $outDir
ls "$baseDir\src\Bugsnag\bin\MonoRelease\*" | %{
	$monoName = "mono-" + $_.Name 
	mv $_ "$outDir\$monoName"
}