#addin "Cake.FileHelpers"

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
        DotNetCoreRestore(solution);
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetCoreBuild(solution, new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            MSBuildSettings = new DotNetCoreMSBuildSettings
            {
                TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error,
                Verbosity = DotNetCoreVerbosity.Minimal
            }
            .SetVersion(version)

            // msbuild.log specified explicitly, see https://github.com/cake-build/cake/issues/1764
            .AddFileLogger(new MSBuildFileLoggerSettings { LogFile = "msbuild.log" })
        });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var projectFiles = GetFiles("./tests/**/*.csproj");

        foreach(var file in projectFiles)
        {
            DotNetCoreTest(file.FullPath);
        }
    });

Task("Publish")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DotNetCorePack("./src/Filter/Filter.csproj", new DotNetCorePackSettings
        {
            Configuration = configuration,
            MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersion(packageVersion),
            NoBuild = true,
            OutputDirectory = publishDirectory
        });

        DotNetCorePack("./src/Range.Web.Http/Range.Web.Http.csproj", new DotNetCorePackSettings
        {
            Configuration = configuration,
            MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersion(packageVersion),
            NoBuild = true,
            OutputDirectory = publishDirectory
        });

        DotNetCorePack("./src/Range.Web.Http.AspNetCore/Range.Web.Http.AspNetCore.csproj", new DotNetCorePackSettings
        {
            Configuration = configuration,
            MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersion(packageVersion),
            NoBuild = true,
            OutputDirectory = publishDirectory
        });

        DotNetCorePack("./src/Filter.Nest/Filter.Nest.csproj", new DotNetCorePackSettings
        {
            Configuration = configuration,
            MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersion(packageVersion),
            NoBuild = true,
            OutputDirectory = publishDirectory
        });

        DotNetCorePack("./src/Filter.NPoco/Filter.NPoco.csproj", new DotNetCorePackSettings
        {
            Configuration = configuration,
            MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersion(packageVersion),
            NoBuild = true,
            OutputDirectory = publishDirectory
        });
    });

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);
