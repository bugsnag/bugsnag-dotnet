Feature: Handled Exceptions are received

Scenario: Notify is called
  Given I configure the bugsnag endpoint
  And I set environment variable "MAZE_API_KEY" to "1234567890"
  When I run the console app "DotNetCore2Console" with "handled"
  Then I should receive a request
  And the request is a valid for the error reporting API
  And the request is valid for a handled exception
