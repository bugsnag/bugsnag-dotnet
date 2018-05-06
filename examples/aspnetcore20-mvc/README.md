# ASP.NET Core 2.0 MVC Example Application

This example shows how you can use the Bugsnag .NET notifier in a basic ASP.NET Core 2.0 MVC app. (We also have an [example](https://github.com/bugsnag/bugsnag-dotnet/tree/master/examples/aspnetcore11-mvc) for ASP.NET Core 1.3)

## Requirements

- Visual Studio 2017
- .NET Core 2.0 development tools for web

## Setup

Try this out with [your own Bugsnag account](https://app.bugsnag.com/user/new)! You'll be able to see how the errors are reported in the dashboard, how breadcrumbs are left, how errors are grouped and how they relate to the original source. Don't forget to replace the placeholder API token with your own!

To get set up, follow the instructions below.

1. Clone the repo and `cd` into this directory:
    ```sh
    git clone https://github.com/bugsnag/bugsnag-dotnet.git
    cd bugsnag-dotnet/examples/aspnetcore20-mvc
    ```

1. In the [Startup.cs](Startup.cs) file Replace the `API_KEY` placeholder with your own Bugsnag API key.

1. In the [aspnetcore20-mvc.csproj](aspnetcore20-mvc.csproj) file, make sure `Bugsnag.AspNet.Core` is included in your NuGet packages, as well as  `Microsoft.AspNetCore.All`.

1. To report non-fatal exceptions, in the [HomeController.cs](Controllers/HomeController.cs) file, make sure to declare the `IClient` dependency where you want the Bugsnag client injected into your classes.

1. Build the application:
    ```sh
    dotnet build
    ```

1. Run the application:
    ```sh
    dotnet run
    ```

1. View the example page which will (most likely) be served at: http://localhost:5000
