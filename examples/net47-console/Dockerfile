FROM microsoft/dotnet-framework-build:4.7.1 AS build-env

WORKDIR /app
COPY packages.config nuget.config ./
RUN ["nuget.exe", "restore", "packages.config", "-PackagesDirectory", "packages"]

COPY . .
RUN msbuild.exe /t:Build /p:Configuration=Release /p:OutputPath=out

FROM microsoft/dotnet-framework:4.7.1
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["net47-console.exe"]
