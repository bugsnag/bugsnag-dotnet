# .NET 8 ASP.NET Core (MVC) Example Application

This example shows how you can use the BugSnag .NET notifier in a basic ASP.NET Core MVC app.

Try this out with [your own BugSnag account](https://app.bugsnag.com/user/new) by replacing the placeholder API key with your own. You'll be able to see how the errors are reported in the dashboard, how breadcrumbs are left, how errors are grouped and how they relate to the original source.

## Requirements

- Visual Studio 2022
- .NET 8.0 SDK

## Setup

The example project is set up to use Bugsnag packages built locally from source. Or, alternatively, you can use Visual Studio to install the latest version from `nuget.org`.

1. Clone the repo and run the build script from the repository root to build and generate the local packages:
    ```sh
    git clone https://github.com/bugsnag/bugsnag-dotnet.git
    cd bugsnag-dotnet
    .\build.ps1 --Target Pack
    ```

1. `cd` into this directory:
    ```sh
    cd examples/aspnetcore8-mvc
    ```

1. In the [Program.cs](Program.cs) file Replace the `API_KEY` placeholder with your own Bugsnag API key.

1. In the [aspnetcore8-mvc.csproj](aspnetcore8-mvc.csproj) file, make sure `Bugsnag.AspNet.Core` is included in your NuGet packages.

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
