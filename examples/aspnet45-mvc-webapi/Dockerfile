# escape=`

FROM microsoft/dotnet-framework-build:4.7.1 AS build-env

# Install web targets
RUN Invoke-WebRequest -UseBasicParsing https://dotnetbinaries.blob.core.windows.net/dockerassets/MSBuild.Microsoft.VisualStudio.Web.targets.2017.12.zip -OutFile MSBuild.Microsoft.VisualStudio.Web.targets.zip;`
    Expand-Archive MSBuild.Microsoft.VisualStudio.Web.targets.zip -DestinationPath \"${Env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\BuildTools\MSBuild\Microsoft\VisualStudio\v15.0\"; `
    Remove-Item -Force MSBuild.Microsoft.VisualStudio.Web.targets.zip

ENV ROSLYN_COMPILER_LOCATION "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\Roslyn"

WORKDIR /app

WORKDIR /app
COPY packages.config nuget.config ./
RUN ["nuget.exe", "restore", "packages.config", "-PackagesDirectory", "packages"]

COPY . .
RUN msbuild.exe /t:Build /p:DeployOnBuild=true /p:PublishProfile=FolderProfile.pubxml

FROM microsoft/aspnet:4.7

WORKDIR /app

COPY --from=build-env /app/bin/PublishOutput /inetpub/wwwroot
