properties {
  # Version (overwritten if built using Appveyor)
  $default_version = "1.1.0.0"
  $version = if ($env:APPVEYOR_BUILD_VERSION) {$env:APPVEYOR_BUILD_VERSION} else {$default_version}
  
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
  
  # Build directories
  $base_dir = resolve-path .
  $src_dir = "$base_dir\..\src"
  $test_dir = "$base_dir\..\test"
  $tools_dir = "$base_dir\..\tools"
  $packages_dir = "$base_dir\..\packages"
  
  # Output directories
  $output_dir = "$base_dir\output"
  $build_dir = "$output_dir\bin"
  $test_out_dir = "$output_dir\test"
  $nuget_dir = "$output_dir\nuget"
}

task default -depends Package, Archive

#------------------------------------------------------------------------
#  PACKAGE
#  Produces Nuget packages for all profiles that have a nuget spec
#------------------------------------------------------------------------
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

    log "Creating Nuget package for $($profile.nuspec)"
    $new_lib_dir = "$nuget_profile_dir\lib"
    mkdir $new_lib_dir | Out-Null
    Copy-Item "$build_dir\$($profile.config)\*" $new_lib_dir -Recurse
    Remove-Item "$new_lib_dir\*" -Exclude "Bugsnag*" -Recurse
    exec { nuget pack $new_spec -OutputDirectory $output_dir -Verbosity quiet}
  }
}

#------------------------------------------------------------------------
#  ARCHIVE                                             
#  Creates a zip file with all compiled binaries (including dependencies) 
#------------------------------------------------------------------------
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

#------------------------------------------------------------------------
#  TEST                                             
#  Runs all xUnit tests for every profile/target framework combination
#------------------------------------------------------------------------
task Test -depends Compile, Clean {
  foreach($project in $projects) {
    foreach($profile in $profiles) {
      log "Running unit tests for $($project.name) ($($profile.config))"
      $test_name = "$($project.name).Test"
      $test_project_file = "$test_dir\$test_name\$test_name.csproj"
      $test_output = "$test_out_dir\$($profile.config)\$($project.name)"
    
      compile_project $test_project_file $profile.config $test_output 
      if ($env:APPVEYOR) {
        exec {& xunit.console.clr4 $("$test_output\$test_name.dll") /noshadow /appveyor}
      } else {      
        exec {& $tools_dir/xunit/xunit.console.clr4.exe $("$test_output\$test_name.dll") /noshadow }
      }
	}
  }
}

#------------------------------------------------------------------------
#  COMPILE                                              
#  Updates Assembly versions and builds all shippable libraries
#------------------------------------------------------------------------
task Compile -depends Clean {
  log "Patching assembly versions to $version"
  replace_line "$src_dir\Common\CommonVersionInfo.cs" "^\[assembly: AssemblyVersion.*$" "[assembly: AssemblyVersion(`"$version`")]"
  replace_line "$src_dir\Common\CommonVersionInfo.cs" "^\[assembly: AssemblyFileVersion.*$" "[assembly: AssemblyFileVersion(`"$version`")]"
  
  foreach($project in $projects) {
    foreach($profile in ($profiles| Where-Object {$_.test_only -ne $true})) {
      log "Building $($project.name) ($($profile.config))"
      $project_file = "$src_dir\$($project.name)\$($project.name).csproj"
      $project_out = "$build_dir\$($profile.config)"
      if ($project.target) {
        $project_out = "$project_out\$($project.target)\"
      }
      compile_project $project_file $profile.config $project_out
    }
  }
}

#------------------------------------------------------------------------
#  CLEAN
#  Removes any build artifacts from previous builds
#------------------------------------------------------------------------
task Clean {
  Remove-Item -force -recurse $output_dir -ErrorAction SilentlyContinue
  log "Deleted previous output directory"
}

#-------------------------------------------------------------------------
#  GLOBAL FUNCTIONS
#-------------------------------------------------------------------------

# Logs text to console (in Yellow)
# - $log_text : The text to show on screen
function global:log($log_text)
{
  Write-Host $log_text -foregroundcolor "Yellow"
}

# Uses MSBuild to compile a .csproj
# - $project_file : Path to the project file
# - $config       : The configuration to use for the build
# - $out_dir      : The directory for the output DLLs and associated files
function global:compile_project($project_file, $config, $out_dir)
{
  $package_file = $(Split-Path $project_file) + $("\packages.config")
  nuget restore $package_file -PackagesDirectory $packages_dir | Out-Null
  exec { msbuild $project_file /p:Configuration=$config /p:OutDir=$out_dir /v:m /nologo }
}

# Replaces lines in a text file
# - $file    : Path to the file
# - $regex   : The regex that will match the lines to replace
# - $newline : The line to replace these matches with
function global:replace_line($file, $regex, $newline)
{
  $content = Get-Content $file
  $content -replace $regex, $newline | Set-Content $file
}
