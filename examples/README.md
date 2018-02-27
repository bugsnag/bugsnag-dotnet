Sample Applications
====

### Requirements

- Windows 10 pro
- Docker for Windows
- Visual Studio 2017
- [.NET framework 3.5](https://docs.microsoft.com/en-us/dotnet/framework/install/dotnet-35-windows-10)

### Setup

For each of the sample applications you'll want to add a Bugsnag API key in the
appropriate place of each sample. Refer to the individual example applications
readme for more information on where to do this for each example.

- [ASP.NET 3.5](aspnet35/)
- [ASP.NET 4.5 MVC & WebApi](aspnet45-mvc-webapi/)
- [ASP.NET Core 1.1 MVC](aspnetcore11-mvc/)
- [ASP.NET Core 2.0 MVC](aspnetcore20-mvc/)
- [.NET 3.5 Console](net35-console/)
- [.NET 4.7 Console](net47-console/)
- [.NET Core 1.1 Console](netcore11-console/)
- [.NET Core 2.0 Console](netcore20-console/)

Run the build script in the root of the repository:
```
.\build.ps1
```
This will compile all of the projects, run the tests, build the required nuget
packages, copy the nuget packages into each of the sample applications and build
the docker images for each sample application.
