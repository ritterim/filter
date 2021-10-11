using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Filter.Tests.Common.Testing.Configuration
{
    public static class ConfigurationHelpers
    {
        public static bool UseDockerDependencies()
        {
            // https://www.appveyor.com/docs/environment-variables/
            
            var useDockerForElasticsearch = bool.TryParse(
                Environment.GetEnvironmentVariable("RIMDEV_CREATE_TEST_DOCKER_ES"),
                out var parseCreateTestDockerElasticsearch
                ) && parseCreateTestDockerElasticsearch;
            
            var useDockerForSql = bool.TryParse(
                Environment.GetEnvironmentVariable("RIMDEV_CREATE_TEST_DOCKER_SQL"),
                out var parseCreateTestDockerSql
                ) && parseCreateTestDockerSql;

            return useDockerForElasticsearch || useDockerForSql;
        }
        
        public static string GetEnvironmentName()
        {
            if (UseDockerDependencies()) return "Docker";
            return Environments.Development;
        }
        
        public static IConfigurationRoot GetConfigurationRoot()
        {
            var environmentName = GetEnvironmentName();
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile($"appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                //.AddUserSecrets("e3dfcccf-0cb3-423a-b302-e3e92e95c128")
                .AddEnvironmentVariables()
                .Build();
        }

        private const string RimDevTestsSectionName = "RimDevTests";
        
        public static RimDevTestsConfiguration GetRimDevTestsConfiguration()
        {
            var configuration = new RimDevTestsConfiguration();
            var configurationRoot = GetConfigurationRoot();
            configurationRoot
                .GetSection(RimDevTestsSectionName)
                .Bind(configuration);
            return configuration;
        }

        public static Uri GetRimDevTestsElasticsearchUri()
        {
            var esConfiguration = GetRimDevTestsConfiguration()?.Elasticsearch
               ?? throw new ArgumentNullException(
                   nameof(RimDevTestsSqlConfiguration),
                   $"Missing {RimDevTestsSectionName} Elasticsearch configuration section."
               );
            return esConfiguration.Uri;
        }

        public static string GetRimDevTestsSqlConnectionString()
        {
            var sqlConfiguration = GetRimDevTestsConfiguration()?.Sql
                ?? throw new ArgumentNullException(
                    nameof(RimDevTestsSqlConfiguration),
                    $"Missing {RimDevTestsSectionName} SQL configuration section."
                    );

            if (string.IsNullOrEmpty(sqlConfiguration?.Password))
                throw new ArgumentNullException(
                    nameof(sqlConfiguration.Password),
                    "No password provided for SQL configuration."
                    );
            
            var builder = new SqlConnectionStringBuilder()
            {
                InitialCatalog = sqlConfiguration?.InitialCatalog,
                DataSource = sqlConfiguration?.DataSource,
                UserID = sqlConfiguration?.UserId,
                Password = sqlConfiguration?.Password,
            };

            return builder.ConnectionString;
        }
    }
}