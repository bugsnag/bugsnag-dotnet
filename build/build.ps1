properties {
  # Custom properties
  $default_version = "1.3.0.0"
  
  # Build profiles
  $profiles = @(
    @{config = "Debug"; test_only=$true},
    @{config = "Release"; nuspec="Bugsnag.nuspec"},
    @{config = "MonoRelease"; nuspec="BugsnagMono.nuspec"}
  )
  
  # Bugsnag Projects
  $projects = @(
    @{name = "Bugsnag"; target = "net45"},
    @{name = "Bugsnag.Net35"; target = "net35"}
  )
  
  # Directories
  $base_dir = resolve-path .
  $output_dir = "$base_dir\output"
  $build_dir = "$output_dir\bin"
  $test_dir = "$output_dir\test"
  $nuget_dir = "$output_dir\nuget"
  
  # Misc directorires
  $src_files_dir = "$base_dir\..\src"
  $test_files_dir = "$base_dir\..\test"
  $tools_dir = "$base_dir\..\tools"
  $packages_dir = "$base_dir\..\packages"
  $sln_file = "$base_dir\..\Bugsnag.sln"
  $version = if ($env:APPVEYOR_BUILD_VERSION) {$env:APPVEYOR_BUILD_VERSION} else {$default_version}
}

task default -depends Package, Archive

task Package -depends Test {
  foreach($profile in ($profiles | Where-Object {$_.nuspec})) {
    $nuget_profile_dir = "$nuget_dir\$($profile.config)"
    mkdir $nuget_profile_dir | Out-Null
    
    log "Updating Nuspec file for $($profile.config) (Version:$version)"
	$template_spec = "$base_dir\$($profile.nuspec)"
    $new_spec = "$nuget_profile_dir\$($profile.nuspec)"	
    Copy-Item $template_spec $new_spec
    $spec_xml = [xml](Get-Content $new_spec)
    $spec_xml.package.metadata.version = $version
    $spec_xml.Save($new_spec)

    log "Creating Nuget package for $($profile.config)"
    $new_lib_dir = "$nuget_profile_dir\lib"
    mkdir $new_lib_dir | Out-Null
    Copy-Item "$build_dir\$($profile.config)\*" $new_lib_dir -Recurse
    Remove-Item "$new_lib_dir\*" -Exclude "Bugsnag*" -Recurse
    exec { nuget pack $new_spec -OutputDirectory $output_dir -Verbosity quiet}
  }
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
  foreach($project in $projects) {
    foreach($profile in $profiles) {
      log "Running unit tests for $($project.name) ($($profile.config))"
      $test_name = "$($project.name).Test"
      $test_project_file = "$test_files_dir\$test_name\$test_name.csproj"
      $test_output = "$test_dir\$($profile.config)\$($project.name)"
    
      compile_project $test_project_file $profile.config $test_output 
      if ($env:APPVEYOR) {
        exec {& xunit.console.clr4 $("$test_output\$test_name.dll") /noshadow /appveyor}
      } else {      
        exec {& $tools_dir/xunit/xunit.console.clr4.exe $("$test_output\$test_name.dll") /noshadow }
      }
	}
  }
}

task Compile -depends Clean {
  log "Patching assembly versions to $version"
  replace_line "$src_files_dir\Common\CommonVersionInfo.cs" "^\[assembly: AssemblyVersion.*$" "[assembly: AssemblyVersion(`"$version`")]"
  replace_line "$src_files_dir\Common\CommonVersionInfo.cs" "^\[assembly: AssemblyFileVersion.*$" "[assembly: AssemblyFileVersion(`"$version`")]"
  
  foreach($project in $projects) {
    foreach($profile in $profiles) {
      if(!$profile.test_only) {
        log "Building $($project.name) ($($profile.config))"
        $project_file = "$src_files_dir\$($project.name)\$($project.name).csproj"
        $project_out = "$build_dir\$($profile.config)"
        if ($project.target) {
          $project_out = "$project_out\$($project.target)\"
        }
        compile_project $project_file $profile.config $project_out
      }
    }
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

function global:compile_project($project_file, $config, $out_dir)
{
  $package_file = $(Split-Path $project_file) + $("\packages.config")
  nuget restore $package_file -PackagesDirectory $packages_dir | Out-Null
  exec { msbuild $project_file /p:Configuration=$config /p:OutDir=$out_dir /v:m /nologo }
}

function global:replace_line($file, $regex, $newline)
{
  $content = Get-Content $file
  $content -replace $regex, $newline | Set-Content $file
}
