# Filter (Development)

## Getting Started

This is a standard .NET Core / .NET 5 project, with a dependency on SQL Server and Elasticsearch for the unit/system tests.

### Software Stack

- [.NET Core 3.1 SDK and/or .NET 5 SDK](https://dotnet.microsoft.com/)
  
### Testing Software

The following software is required to be available in order to run the unit/system tests.

- [Microsoft SQL Server Developer](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or Docker) to run the unit/system tests.
- [Elasticsearch 6.x](https://www.elastic.co/downloads/elasticsearch) (or Docker) to run the unit/system tests.

We recommend using Docker for these dependencies for ease of use. 

## Test Configuration

By default, the unit/system tests assume that you have:

- Microsoft SQL Server (nearly any variant) running on localhost,1433 with a no-password 'sa' account.
- Elasticsearch 6.x running on http://localhost:9200.

However, if you have Docker installed, you can just run `build.cmd` or `build.sh` to build/test the project.

## Customizing Test Configuration

Because the actual port/password will vary according to the developer's personal machine settings, it is possible to override these configuration values when running the unit/system tests.

### appsettings.Development.json
    
You can create `appsettings.Development.json` files under each of the xUnit test projects to customize the settings for communication with SQL/Elasticsearch.  In addition, these files must be added to the `.csproj` file and have the `<CopyToOutputDirectory>Always</CopyToOutputDirectory>` property applied.

### Environment Variables

Because we follow the .NET Core configuration convention where environment variables can override portions of the appsettings.json value, you can set environment variables on your development machine to configure these values.  Note that by convention, you need to use double-underscores between levels in the JSON hierarchy.  

For JetBrains Rider users, take a look at "Preferences" -> "Test Runner -> "Environment Variables".

#### Elasticsearch

- `RIMDEVTESTS__ELASTICSEARCH__BASEURI="http://localhost"`
- `RIMDEVTESTS__ELASTICSEARCH__PORT="9201"`
- `RIMDEVTESTS__ELASTICSEARCH__TRANSPORTPORT="9301"`

#### SQL Server

- `RIMDEVTESTS__SQL__HOSTNAME="localhost"` 
- `RIMDEVTESTS__SQL__PORT="11433"`
- `RIMDEVTESTS__SQL__USERID="sa"` 
- `RIMDEVTESTS__SQL__PASSWORD="your-local-sa-password"` 
