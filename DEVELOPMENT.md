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

## Customizing Test Configuration

Because the actual port/password will vary according to the developer's personal machine settings, it is possible to override these configuration values when running the unit/system tests.

#### appsettings.Development.json

The appsettings.Development.json files under each of the xUnit test projects can be edited in order to customize how the unit test fixtures will communicate with SQL/Elasticsearch.  These files are empty by default.

#### Environment Variables

Because we follow the .NET Core configuration convention where environment variables can override portions of the appsettings.json value, you can set environment variables on your development machine to configure these values.  Note that by convention, you need to use double-underscores between levels in the JSON hierarchy.

- `RimDevTests__Sql__DataSource="localhost,11433"` 
- `RimDevTests__Sql__UserId="sa"` 
- `RimDevTests__Sql__Password="your-local-sa-password"` 
- `RimDevTests__Elasticsearch__Uri="http://localhost:9201"`



