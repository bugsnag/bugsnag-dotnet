#!/usr/bin/env bash

mono .nuget/NuGet.exe install FAKE -OutputDirectory packages -ExcludeVersion

mono packages/FAKE/tools/Fake.exe build.fsx
