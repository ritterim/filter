using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Filter.Tests.Common.Testing.Configuration
{
    public static class ConfigurationHelpers
    {
        /// <summary>Figure out which additional environment-specific appsettings.json
        /// to load over top of the base appsettings.json file settings.</summary>
        public static string GetRimDevTestEnvironmentName()
        {
            var rimDevTestEnvironment = Environment.GetEnvironmentVariable("RIMDEVTEST_ENVIRONMENT");
            return string.IsNullOrWhiteSpace(rimDevTestEnvironment) 
                ? Environments.Development 
                : rimDevTestEnvironment;
        }
        
        public static IConfigurationRoot GetConfigurationRoot()
        {
            var baseDirectory = AppContext.BaseDirectory;
            Console.WriteLine($"Looking for appsettings JSON files in '{baseDirectory}'.");
            
            var environmentName = GetRimDevTestEnvironmentName();
            Console.WriteLine($"Loading appsettings.json and appsettings.{environmentName}.json.");

            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile($"appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
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