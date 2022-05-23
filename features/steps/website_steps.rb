Given("I configure the bugsnag endpoint") do
  steps %Q{
    When I set environment variable "MAZE_ENDPOINT" to "http://localhost:#{MOCK_API_PORT}"
  }
end

When("I run the console app {string} with {string}") do |name, args|
  path = File.join(Dir.pwd, "features", "fixtures", name)
  nuget_package_path = File.join(Dir.pwd, "build", "packages")
  run_command("dotnet add #{path} package bugsnag --version #{ENV['BUGSNAG_VERSION']} --no-restore")
  run_command("dotnet restore --source #{nuget_package_path} --no-cache")
  run_command(@script_env || {}, "dotnet run -p #{path} -- #{args}")
end

Then("the request is valid for a handled exception") do
  steps %Q{
      Then the event "unhandled" is false
      And the event "severity" equals "warning"
      And the event "severityReason.type" equals "handledException"
    }
end
