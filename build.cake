#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = "./Filter.sln";
var publishDirectory = Directory("./artifacts");

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(publishDirectory);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        NuGetRestore(solution);
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        MSBuild(solution, settings =>
            settings.SetConfiguration(configuration)
                .WithProperty("TreatWarningsAsErrors", "False")
                .SetVerbosity(Verbosity.Minimal)
                .AddFileLogger());
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        XUnit2("./tests/**/bin/" + configuration + "/*.Tests.dll", new XUnit2Settings());
    });

Task("Publish")
    .IsDependentOn("Test")
    .Does(() =>
    {
        NuGetPack("./src/Filter/Filter.csproj", new NuGetPackSettings
        {
            OutputDirectory = publishDirectory,
            Version = EnvironmentVariable("GitVersion_NuGetVersionV2"),
            Properties = new Dictionary<string, string>
            {
                { "Configuration", configuration }
            }
        });

        NuGetPack("./src/Range.Web.Http/Range.Web.Http.csproj", new NuGetPackSettings
        {
            OutputDirectory = publishDirectory,
            Version = EnvironmentVariable("GitVersion_NuGetVersionV2"),
            Properties = new Dictionary<string, string>
            {
                { "Configuration", configuration }
            }
        });

        NuGetPack("./src/Filter.Nest/Filter.Nest.csproj", new NuGetPackSettings
        {
            OutputDirectory = publishDirectory,
            Version = EnvironmentVariable("GitVersion_NuGetVersionV2"),
            Properties = new Dictionary<string, string>
            {
                { "Configuration", configuration }
            }
        });
    });

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);