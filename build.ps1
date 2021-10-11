New-Item -ItemType directory -Path "build" -Force | Out-Null

# Release whenever a commit is merged to master
$ENV:UseMasterReleaseStrategy = "true"

# The following variables should be set to true if unit tests need a Docker container created
$ENV:RIMDEV_CREATE_TEST_DOCKER_ES = "true" # Elasticsearch on 9201/9301
$ENV:RIMDEV_CREATE_TEST_DOCKER_SQL = "true" # MS SQL on 11433/11434
$ENV:RIMDEVTESTS__SQL__PASSWORD = "MvKbeUn18mIxc0r" # This password is used for the temporary Docker container

# backwards compatibility for build-net5.cake file
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
