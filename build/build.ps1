properties {
  # Custom properties
  $default_version = 6.7
  $config = "Debug"
  
  # Bugsnag Projects
  $projects = @(
    @{name = "Bugsnag"; target = "4.5"},
    @{name = "Bugsnag.Net35"; target = "3.5"}
  )
  
  # Directories
  $base_dir = resolve-path .
  $output_dir = "$base_dir\output"
  $build_dir = "$output_dir\bin"
  $test_dir = "$output_dir\test"
  
  # Nuget Directories
  $nuget_dir = "$output_dir\nuget"
  $nuget_lib_dir = "$nuget_dir\lib"
  $nuget_spec_template = "$base_dir\Bugsnag.nuspec"
  $nuget_spec = "$nuget_dir\Bugsnag.nuspec"
  
  # Misc directorires
  $tools_dir = "$base_dir\..\tools"
  $packages_dir = "$base_dir\..\packages"
  $sln_file = "$base_dir\..\Bugsnag.sln"
  $version = if ($env:APPVEYOR_BUILD_VERSION) {$env:APPVEYOR_BUILD_VERSION} else {$default_version}
}

task default -depends Package, Archive

task Package -depends Test {
	mkdir $nuget_dir | Out-Null
	
	log "Updating Nuspec file (Version:$version)" 
	Copy-Item $nuget_spec_template $nuget_spec
	$spec = [xml](Get-Content $nuget_spec )
    $spec.package.metadata.version = ([string]$version)
    $spec.Save($nuget_spec)

	log "Creating Nuget package" 
    mkdir $nuget_lib_dir | Out-Null
    Copy-Item "$build_dir\*" $nuget_lib_dir -Recurse
	Remove-Item "$nuget_lib_dir\*" -Exclude "Bugsnag.*" -Recurse
    exec { nuget pack $nuget_spec -OutputDirectory $output_dir }
}

task Archive -depends Test {
  if ($env:APPVEYOR) {
	log "Skipping archiving, Appveyor will publish archive as artifact"
  } else {
    log "Creating zip file of binaries"
    mkdir "$output_dir\archive" | Out-Null   
    Copy-Item $build_dir "$output_dir\archive" -Recurse
    Write-Zip -Path "$output_dir\archive\*" -OutputPath "$output_dir\bugsnag.$version.zip" | Out-Null  
	Remove-Item "$output_dir\archive" -Recurse
  }
}

task Test -depends Compile, Clean {
  if ($env:APPVEYOR) {
	log "Skipping unit tests, Appveyor will run tests"
  } else {
	foreach($project in $projects) {
	  log "Running unit tests for $($project.name)"
	  $test_name = "$($project.name).Test"
	  $test_project_file = "..\test\$test_name\$test_name.csproj"
	  $test_output = "$test_dir\$($project.target)"
	  
	  compile_project $test_project_file $test_output 
	  exec {& $tools_dir/xunit/xunit.console.clr4.exe $("$test_output\$test_name.dll") /noshadow }
	}
  }
}

task Compile -depends Clean {
  foreach($project in $projects) {
    log "Building $($project.name)"
	$project_file = "..\src\$($project.name)\$($project.name).csproj"
	$project_out = "$build_dir\$($project.target)\"
	compile_project $project_file $project_out
  }
}

task Clean {
  Remove-Item -force -recurse $output_dir -ErrorAction SilentlyContinue
  log "Deleted previous output directory"
}
#-------------------------------------------------------------------------
#  FUNCTIONS
#-------------------------------------------------------------------------

function global:log($log_text)
{
  Write-Host $log_text -foregroundcolor "Yellow"
}

function global:compile_project($project_file, $out_dir)
{
  $package_file = $(Split-Path $project_file) + $("\packages.config")
  nuget restore $package_file -PackagesDirectory $packages_dir | Out-Null
  exec { msbuild $project_file /p:Configuration=$config /p:OutDir=$out_dir /v:m /nologo }
}
