#!/bin/bash
rm -r ./nupkgs
dotnet pack --output "nupkgs" --configuration Release --include-symbols --include-source -p:RepositoryUrl=https://github.com/designcrowd/SlackAPI
dotnet nuget push nupkgs/*.symbols.nupkg -s GitHub
