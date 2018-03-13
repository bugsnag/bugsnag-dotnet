FROM microsoft/dotnet-framework-build:3.5 AS build-env

WORKDIR /app
COPY packages.config nuget.config ./
RUN ["nuget.exe", "restore", "packages.config", "-PackagesDirectory", "packages"]

COPY . .
RUN msbuild.exe /t:Build /p:Configuration=Release /p:OutputPath=out

FROM microsoft/dotnet-framework:3.5
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["net35-console.exe"]
