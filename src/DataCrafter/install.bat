@echo off

REM Run 'dotnet pack'
dotnet pack

REM Run 'dotnet tool uninstall --global DataCrafter'
dotnet tool uninstall --global DataCrafter

REM Run 'dotnet tool install --global --add-source ./nupkg DataCrafter'
dotnet tool install --global --add-source ./nupkg DataCrafter