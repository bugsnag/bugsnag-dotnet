properties {
  $base_dir = resolve-path .
  $build_dir = "$base_dir\bin"
  $test_dir = "$base_dir\test"
  $tools_dir = "$base_dir\..\tools"
  $packageinfo_dir = "$base_dir\packaging"
  $45_build_dir = "$build_dir\4.5\"
  $35_build_dir = "$build_dir\3.5\"
  $45_test_dir = "$test_dir\4.5\"
  $35_test_dir = "$test_dir\3.5\"
  $mono_build_dir = "$build_dir\Mono\"
  $release_dir = "$base_dir\Release"
  $sln_file = "$base_dir\..\Bugsnag.sln"
  $version = 5.2
  $config = "Debug"
  $run_tests = $true
}

task default -depends Test

task Test -depends Compile, Clean {
  $old = pwd
  cd $base_dir
  & $tools_dir/xunit/xunit.console.clr4.exe "$45_test_dir/Bugsnag.Test.dll" /noshadow
  & $tools_dir/xunit/xunit.console.clr4.exe "$35_test_dir/Bugsnag.Net35.Test.dll" /noshadow
  cd $old
}

task Compile -depends Clean {
  msbuild ../src/Bugsnag/Bugsnag.csproj /p:Configuration=$config /p:OutDir=$45_build_dir /v:m /nologo
  msbuild ../test/Bugsnag.Test/Bugsnag.Test.csproj /p:Configuration=$config /p:OutDir=$45_test_dir /v:m /nologo

  msbuild ../src/Bugsnag.Net35/Bugsnag.Net35.csproj /p:Configuration=$config /p:OutDir=$35_build_dir /v:m /nologo
  msbuild ../test/Bugsnag.Net35.Test/Bugsnag.Net35.Test.csproj /p:Configuration=$config /p:OutDir=$35_test_dir /v:m /nologo
}

task Clean {
  remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue
  Write-Host "Deleted " $build_dir
  remove-item -force -recurse $test_dir -ErrorAction SilentlyContinue
  Write-Host "Deleted " $test_dir
}
