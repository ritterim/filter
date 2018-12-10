#addin "Cake.FileHelpers"
#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = "./Filter.sln";
var publishDirectory = Directory("./artifacts");

var version = FileReadText("./version.txt").Trim();

var packageVersion = version;
if (!AppVeyor.IsRunningOnAppVeyor)
{
    packageVersion += "-dev";
}

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

Task("Version")
    .Does(() =>
    {
        foreach (var assemblyInfo in GetFiles("./src/**/AssemblyInfo.cs"))
        {
            CreateAssemblyInfo(
                assemblyInfo.ChangeExtension(".Generated.cs"),
                new AssemblyInfoSettings
                {
                    Version = version,
                    InformationalVersion = packageVersion
                });
        }
    });

Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("Version")
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
            Properties = new Dictionary<string, string>
            {
                { "Configuration", configuration }
            }
        });

        NuGetPack("./src/Range.Web.Http/Range.Web.Http.csproj", new NuGetPackSettings
        {
            OutputDirectory = publishDirectory,
            Properties = new Dictionary<string, string>
            {
                { "Configuration", configuration }
            }
        });

        NuGetPack("./src/Filter.Nest/Filter.Nest.csproj", new NuGetPackSettings
        {
            OutputDirectory = publishDirectory,
            Properties = new Dictionary<string, string>
            {
                { "Configuration", configuration }
            }
        });

        NuGetPack("./src/Filter.NPoco/Filter.NPoco.csproj", new NuGetPackSettings
        {
            OutputDirectory = publishDirectory,
            Properties = new Dictionary<string, string>
            {
                { "Configuration", configuration }
            }
        });
    });

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);