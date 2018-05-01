Given("I configure the bugsnag endpoint") do
  steps %Q{
    When I set environment variable "MAZE_ENDPOINT" to "http://localhost:#{MOCK_API_PORT}"
  }
end

When("I run the console app {string} with {string}") do |name, args|
  path = File.join(Dir.pwd, "features", "fixtures", name)
  nuget_package_path = File.join(Dir.pwd, "build", "packages")
  run_command("dotnet nuget locals all --clear")
  run_command("dotnet add #{path} package -v #{ENV['BUGSNAG_VERSION']} bugsnag")
  run_command(@script_env || {}, "dotnet run -p #{path} -- #{args}")
end
