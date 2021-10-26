New-Item -ItemType directory -Path "build" -Force | Out-Null

# Release whenever a commit is merged to master
$ENV:UseMasterReleaseStrategy = "true"

# The following variables should be set to true if unit tests need a Docker container created.
# Configuration logic in the build/tests will assume that you want to talk to the Docker instance.
$ENV:RIMDEV_CREATE_TEST_DOCKER_ES = "true" # Elasticsearch
$ENV:RIMDEV_CREATE_TEST_DOCKER_SQL = "true" # MS SQL

if ( $ENV:RIMDEV_CREATE_TEST_DOCKER_ES )
{
    $ENV:RIMDEVTESTS__ELASTICSEARCH__BASEURI = "http://localhost"
    $ENV:RIMDEVTESTS__ELASTICSEARCH__PORT = "9202"
    $ENV:RIMDEVTESTS__ELASTICSEARCH__TRANSPORTPORT = "9302"
}

if ( $ENV:RIMDEV_CREATE_TEST_DOCKER_SQL )
{
    $ENV:RIMDEVTESTS__SQL__HOSTNAME = "localhost"
    $ENV:RIMDEVTESTS__SQL__PORT = "11435"
    $ENV:RIMDEVTESTS__SQL__PASSWORD = "MvKbeUn18mIxc0r"
}

# backwards compatibility for existing build-net5.cake file
$ENV:RIMDEV_TEST_DOCKER_MSSQL_SA_PASSWORD = $ENV:RIMDEVTESTS__SQL__PASSWORD

try {
  $BootstrapResponse = Invoke-WebRequest https://raw.githubusercontent.com/ritterim/build-scripts/master/bootstrap-cake.ps1 -OutFile build\bootstrap-cake.ps1
  $ScriptResponse = Invoke-WebRequest https://raw.githubusercontent.com/ritterim/build-scripts/master/build-net5.cake -OutFile build.cake
}
catch {
  Write-Output $_.Exception.Message
  Write-Output "Error while downloading shared build script, attempting to use previously downloaded scripts..."
}

#.\build\bootstrap-cake.ps1 -Verbose --verbosity=Normal
.\build\bootstrap-cake.ps1 -Verbose --verbosity=Diagnostic

Exit $LastExitCode
