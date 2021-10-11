#addin nuget:?package=Cake.FileHelpers&version=3.2.1
#addin nuget:?package=Cake.Npm&version=0.16.0
#addin nuget:?package=Cake.Docker&version=0.11.1
#tool nuget:?package=xunit.runner.console&version=2.4.1

Information("build-net5.cake -- Jul-26-2021");
var target = Argument("target", "Default");

// RELEASE STRATEGY: old vs new git flow (master branch vs trunk-based release strategy)
//   false (default) = only "release/*" branches result in production artifacts
//   true = any commit to master results in production artifacts
// Any git repo that wants to release on master, must set environment var: UseMasterReleaseStrategy=true
var useMasterReleaseStrategy = true;
bool.TryParse(EnvironmentVariable("UseMasterReleaseStrategy"), out useMasterReleaseStrategy);
Information($"useMasterReleaseStrategy={useMasterReleaseStrategy}");

// Calculate the version
var versionFromFile = FileReadText("./version.txt").Trim().Split('.')
    .Take(2).Aggregate("", (version, x) => $"{version}{x}.").Trim('.');
var buildNumber = AppVeyor.Environment.Build.Number;
var version = $"{versionFromFile}.{buildNumber}";
Information($"version={version}");

var packageVersion = version;
if (!AppVeyor.IsRunningOnAppVeyor)
{
    packageVersion += "-dev";
}
else if ((useMasterReleaseStrategy && AppVeyor.Environment.Repository.Branch != "master")
    || (!useMasterReleaseStrategy && !AppVeyor.Environment.Repository.Branch.StartsWith("release/")))
{
    packageVersion += "-alpha";
}
Information($"packageVersion={packageVersion}");

var configuration = "Release";
Information($"configuration={configuration}");

var artifactsDir = Directory("./artifacts");

// Assume a single solution per repository
var solution = GetFiles("./**/*.sln").First().ToString();
Information($"solution={solution}");

// Look for a "host" project (named either "host" or "api")
var hostProject = GetFiles("./src/**/*.csproj")
    .SingleOrDefault(x =>
        (
            x.GetFilename().FullPath.ToLowerInvariant().Contains("api")
            || x.GetFilename().FullPath.ToLowerInvariant().Contains("host")
        )
        && !x.GetFilename().FullPath.ToLowerInvariant().Contains("webapi"));
Information($"hostProject={hostProject}");
var hostDirectory = hostProject?.GetDirectory();
Information($"hostDirectory={hostDirectory}");
var npmPackageLockFile = (hostDirectory != null)
    ? GetFiles($"{hostDirectory}/package-lock.json").SingleOrDefault()
    : null;
Information($"npmPackageLockFile={npmPackageLockFile}");

var createElasticsearchDocker = false;
bool.TryParse(EnvironmentVariable("RIMDEV_CREATE_TEST_DOCKER_ES"), out createElasticsearchDocker);
Information($"createElasticsearchDocker={createElasticsearchDocker}");

var createSqlDocker = false;
bool.TryParse(EnvironmentVariable("RIMDEV_CREATE_TEST_DOCKER_SQL"), out createSqlDocker);
Information($"createSqlDocker={createSqlDocker}");

var DockerSqlName = "test-mssql";
var DockerElasticsearchName = "test-es";

Setup(context =>
{
    Information("Starting up Docker test container(s).");

    // Output a list of the pre-installed docker images on the AppVeyor instance.
    // This could help us pick images that do not have to be downloaded on every run.
    // Requires build.ps1 to call Cake with --verbosity=Diagnostic
    DockerImageLs(new DockerImageLsSettings());

    if (createSqlDocker)
    {
        var sqlDockerId = DockerPs(new DockerContainerPsSettings
        {
            All = true,
            Filter = $"name={DockerSqlName}",
            Quiet = true,
        });
        Information($"Existing sqlDockerId={sqlDockerId}");
        if (sqlDockerId != "") DockerStop(sqlDockerId);

        // https://hub.docker.com/_/microsoft-mssql-server
        DockerRun(new DockerContainerRunSettings
        {
            Detach = true,
            Env = new string[]
            {
                "ACCEPT_EULA=Y",
                $"SA_PASSWORD={EnvironmentVariable("RIMDEV_TEST_DOCKER_MSSQL_SA_PASSWORD")}",
            },
            Name = DockerSqlName,
            Publish = new string[]
            {
                "11434:1433",
            },
            Rm = true,
        },
        "mcr.microsoft.com/mssql/server:2019-latest",
        null,
        null
        );

        sqlDockerId = DockerPs(new DockerContainerPsSettings
        {
            All = true,
            Filter = $"name={DockerSqlName}",
            Quiet = true,
        });
        Information($"Created sqlDockerId={sqlDockerId}");
    }

    if (createElasticsearchDocker)
    {
        var elasticsearchDockerId = DockerPs(new DockerContainerPsSettings
        {
            All = true,
            Filter = $"name={DockerElasticsearchName}",
            Quiet = true,
        });
        Information($"Existing elasticsearchDockerId={elasticsearchDockerId}");
        if (elasticsearchDockerId != "") DockerStop(elasticsearchDockerId);

        // https://hub.docker.com/_/elasticsearch
        DockerRun(new DockerContainerRunSettings
        {
            Detach = true,
            Env = new string[]
            {
                "discovery.type=single-node",
                "ES_JAVA_OPTS=-Xms256m -Xmx256m",
            },
            Name = DockerElasticsearchName,
            Publish = new string[]
            {
                "9201:9200",
                "9301:9300",
            },
            Rm = true,
        },
        "docker.elastic.co/elasticsearch/elasticsearch:6.8.13",
        null,
        null
        );

        elasticsearchDockerId = DockerPs(new DockerContainerPsSettings
        {
            All = true,
            Filter = $"name={DockerElasticsearchName}",
            Quiet = true,
        });
        Information($"Created elasticsearchDockerId={elasticsearchDockerId}");
    }

    DockerPs(new DockerContainerPsSettings
    {
        All = true,
        NoTrunc = true,
        Size = true,
    });
});

Teardown(context =>
{
    Information("Stopping Docker test container(s).");

    var sqlDockerId = DockerPs(new DockerContainerPsSettings
    {
        All = true,
        Filter = $"name={DockerSqlName}",
        Quiet = true,
    });
    Information($"Found sqlDockerId={sqlDockerId}");
    if (sqlDockerId != "") DockerStop(sqlDockerId);

    var elasticsearchDockerId = DockerPs(new DockerContainerPsSettings
    {
        All = true,
        Filter = $"name={DockerElasticsearchName}",
        Quiet = true,
    });
    Information($"Found sqlDockerId={sqlDockerId}");
    if (elasticsearchDockerId != "") DockerStop(elasticsearchDockerId);
});

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDir);
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore(solution);
    });

Task("Restore-npm-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        if (hostDirectory is null || npmPackageLockFile is null) return;

        Information($"Found NPM package-lock.json.");
        NpmCi(new NpmCiSettings
        {
            LogLevel = NpmLogLevel.Warn,
            WorkingDirectory = hostDirectory
        });
    });

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Restore-npm-Packages")
    .Does(() =>
    {
        if (hostDirectory != null && npmPackageLockFile != null)
        {
            NpmRunScript(new NpmRunScriptSettings
            {
                ScriptName = "webpack",
                WorkingDirectory = hostDirectory
            });
        }

        DotNetCoreBuild(solution, new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            MSBuildSettings = new DotNetCoreMSBuildSettings
            {
                TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error,
                Verbosity = DotNetCoreVerbosity.Minimal
            }
            .SetVersion(packageVersion)

            // msbuild.log specified explicitly, see https://github.com/cake-build/cake/issues/1764
            .AddFileLogger(new MSBuildFileLoggerSettings { LogFile = "msbuild.log" })
        });
    });

Task("Run-Tests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        if (AppVeyor.IsRunningOnAppVeyor)
        {
            DockerPs(new DockerContainerPsSettings());
        }

        var projectFiles = GetFiles("./tests/**/*.csproj");
        foreach (var file in projectFiles)
        {
            DotNetCoreTest(file.FullPath);
        }
    });

Task("Package")
    .IsDependentOn("Run-Tests")
    .Does(() =>
    {
        if (hostProject != null)
        {
            Information($"Found a host/api project to build.");

            var hostArtifactsDir = artifactsDir + Directory("Host");
            Information($"hostArtifactsDir={hostArtifactsDir}");

            var hostProjectName = hostProject.GetFilenameWithoutExtension();
            Information($"hostProjectName={hostProjectName}");

            DotNetCorePublish(hostProject.ToString(), new DotNetCorePublishSettings
            {
                Configuration = configuration,
                OutputDirectory = hostArtifactsDir,
                MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersion(packageVersion)
            });

            // add a githash.txt file to the host output directory (must be after DotNetCorePublish)
            System.IO.File.AppendAllText(
                hostArtifactsDir + File("githash.txt"),
                BuildSystem.AppVeyor.Environment.Repository.Commit.Id);

            // work around for datetime offset problem
            var now = DateTime.UtcNow;
            foreach(var file in GetFiles($"{hostArtifactsDir}/**/*.*"))
            {
                System.IO.File.SetLastWriteTimeUtc(file.FullPath, now);
            }

            Zip(
                hostArtifactsDir,
                "./artifacts/" + hostProjectName + ".zip"
            );
        }

        // Search for class library DLLs that need to be published to NuGet/MyGet.
        // They must have PackageId defined in the .csproj file.
        Information("\nSearching for csproj files with PackageId defined to create NuGet packages...");
        var clientProjects = GetFiles("./src/**/*.csproj");
        foreach (var clientProject in clientProjects)
        {
            var clientProjectPath = clientProject.ToString();
            Information($"\nclientProjectPath={clientProjectPath}");

            // XmlPeek - https://stackoverflow.com/a/34886946
            var packageId = XmlPeek(
                clientProjectPath,
                "/Project/PropertyGroup/PackageId/text()",
                new XmlPeekSettings { SuppressWarning = true }
                );
            Information($"packageId={packageId}");

            if (!string.IsNullOrWhiteSpace(packageId))
            {
                DotNetCorePack(clientProjectPath, new DotNetCorePackSettings
                {
                    Configuration = configuration,
                    MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersion(packageVersion),
                    NoBuild = true,
                    OutputDirectory = artifactsDir,
                    IncludeSymbols = true,

                    //TODO: Remove ArgumentCustomization, add SymbolPackageFormat once Cake 1.2 is released
                    // https://github.com/cake-build/cake/pull/3331
                    ArgumentCustomization = x => x.Append("-p:SymbolPackageFormat=snupkg")
                    //SymbolPackageFormat = "snupkg",
                });
            }
        }
    });

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);
