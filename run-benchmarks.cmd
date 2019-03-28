@echo Off
pushd %~dp0
setlocal enabledelayedexpansion

dotnet build --configuration Release
dotnet tests\Benchmarks\bin\Release\netcoreapp2.2\Benchmarks.dll
