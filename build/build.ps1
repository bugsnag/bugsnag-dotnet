properties {
  $base_dir = resolve-path .
  
  # Build directories
  $build_dir = "$base_dir\bin"
  $45_build_dir = "$build_dir\4.5\"
  $35_build_dir = "$build_dir\3.5\"
  
  # Test directories
  $test_dir = "$base_dir\test"
  $45_test_dir = "$test_dir\4.5\"
  $35_test_dir = "$test_dir\3.5\"
  
  $archive_dir = "$base_dir\archive"
  $nuget_dir = "$base_dir\nuget"
  $nuget_spec_template = "$base_dir\Bugsnag.nuspec"
  $nuget_spec = "$base_dir\nuget\Bugsnag.nuspec"
  
  $tools_dir = "$base_dir\..\tools"
  $packageinfo_dir = "$base_dir\packaging"
  $sln_file = "$base_dir\..\Bugsnag.sln"
  $version = 5.2
  $config = "Debug"
}
import-module Pscx

task default -depends Package, Archive

task Package -depends Test {
	mkdir $nuget_dir | Out-Null
	Copy-Item $nuget_spec_template $nuget_spec

    mkdir "$nuget_dir\lib" | Out-Null
    Copy-Item "$build_dir\*" "$nuget_dir\lib" -Recurse

    $spec = [xml](Get-Content $nuget_spec )
    $spec.package.metadata.version = ([string]$version)
    $spec.Save($nuget_spec)

    #exec { nuget pack "$NuGetPackDir\$NuSpecFileName" }
}
task Archive -depends Test {
    mkdir $archive_dir | Out-Null   
    Copy-Item $build_dir $archive_dir -Recurse
    Write-Zip -Path "$archive_dir\*" -OutputPath bugsnag.zip | Out-Null  
	Remove-Item "$archive_dir\*" -Recurse
	Move-Item  bugsnag.zip "$archive_dir\bugsnag.zip" 
}

task Test -depends Compile, Clean {
  Write-Host "Compiling Test Assemblies"
  nuget restore ../test/Bugsnag.Test/packages.config -PackagesDirectory ../packages
  nuget restore ../test/Bugsnag.Net35.Test/packages.config -PackagesDirectory ../packages
  exec { msbuild ../test/Bugsnag.Test/Bugsnag.Test.csproj /p:Configuration=$config /p:OutDir=$45_test_dir /v:m /nologo } 
  exec { msbuild ../test/Bugsnag.Net35.Test/Bugsnag.Net35.Test.csproj /p:Configuration=$config /p:OutDir=$35_test_dir /v:m /nologo}

  Write-Host "Executing Tests" -foregroundcolor "Yellow"
  $old = pwd
  cd $base_dir
  exec {& $tools_dir/xunit/xunit.console.clr4.exe "$45_test_dir/Bugsnag.Test.dll" /noshadow | select-string -notmatch -pattern "xunit.dll","xUnit.net","Copyright","Tests complete","^\s*$"  -casesensitive}
  exec {& $tools_dir/xunit/xunit.console.clr4.exe "$35_test_dir/Bugsnag.Net35.Test.dll" /noshadow | select-string -notmatch -pattern "xunit.dll","xUnit.net","Copyright","Tests complete","^\s*$"   -casesensitive}
  cd $old
}

task Compile -depends Clean {
  Write-Host "Compiling Build Assemblies"
  nuget restore ../src/Bugsnag/packages.config -PackagesDirectory ../packages
  nuget restore ../src/Bugsnag.Net35/packages.config -PackagesDirectory ../packages
  exec { msbuild ../src/Bugsnag/Bugsnag.csproj /p:Configuration=$config /p:OutDir=$45_build_dir /p:AssemblyName=Bugsnag /v:m /nologo }
  exec { msbuild ../src/Bugsnag.Net35/Bugsnag.Net35.csproj /p:Configuration=$config /p:OutDir=$35_build_dir /p:AssemblyName=Bugsnag /v:m /nologo}
}

task Clean {
  remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue
  Write-Host "Deleted " $build_dir
  remove-item -force -recurse $test_dir -ErrorAction SilentlyContinue
  Write-Host "Deleted " $test_dir
  remove-item -force -recurse $archive_dir -ErrorAction SilentlyContinue
  Write-Host "Deleted " $archive_dir
  remove-item -force -recurse $nuget_dir -ErrorAction SilentlyContinue
  Write-Host "Deleted " $nuget_dir
}

remove-module Pscx