using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Filter.Tests.Common.Testing;
using Filter.Tests.Common.Testing.Configuration;
using NPoco;
using NPoco.SqlServer;

namespace Filter.NPoco.Tests.Testing
{
    /// <summary>A database fixture for use in tests where all of the migrations have been run. 
    /// </summary>
    public class NPocoDatabaseFixture : TestSqlClientDatabaseFixture
    {
        public NPocoDatabaseFixture() : base (
            ConfigurationHelpers.GetRimDevTestsSqlConnectionString(), 
            deleteBefore: true,
            deleteAfter: true,
            dbName: null // this will create a new randomized database name each time
            )
        {
            Console.WriteLine($"Creating {nameof(NPocoDatabaseFixture)} instance.");
            Console.WriteLine(MasterConnectionStringDebug);
            Console.WriteLine(ConnectionStringDebug);

            ExecuteMigrations();
            InsertTestData();
            
            Console.WriteLine($"{nameof(NPocoDatabaseFixture)} instance created.");
        }

        private void ExecuteMigrations()
        {
            Console.WriteLine("Executing migrations.");
            
            using var connection = CreateSqlConnection();
            connection.Open();
            
            CreatePersonTable(connection);
            
            Console.WriteLine("Finished migrations.");
        }

        private void CreatePersonTable(SqlConnection connection)
        {
            using var command = new SqlCommand();
            
            command.Connection = connection;
            command.CommandText = @"
create table Person(
    Id int identity not null,
    DONOTUSE nvarchar(50),
    FavoriteDate datetime,
    FavoriteDateTimeOffset datetimeoffset,
    FavoriteLetter nchar(1),
    FavoriteNumber int,
    FirstName nvarchar(50),
    LastName nvarchar(50),
    Rating decimal(5,2) null
                )";

            command.ExecuteNonQuery();
        }
        
        private readonly IEnumerable<Person> people = new List<Person>()
        {
            new Person()
            {
                FavoriteDate = DateTime.Parse("2000-01-01"),
                FavoriteDateTimeOffset = DateTimeOffset.Parse("2010-01-01"),
                FavoriteLetter = 'a',
                FavoriteNumber = 5,
                FirstName = "John",
                LastName = "Doe",
                Rating = null
            },
            new Person()
            {
                FavoriteDate = DateTime.Parse("2000-01-02"),
                FavoriteDateTimeOffset = DateTimeOffset.Parse("2010-01-02"),
                FavoriteLetter = 'b',
                FavoriteNumber = 10,
                FirstName = "Tim",
                LastName = "Smith",
                Rating = 4.5m
            },
        };
        
        private void InsertTestData()
        {
            Console.WriteLine("Inserting test data.");
            
            using var db = GetDatabaseInstance();
            db.InsertBulk(people);
            
            Console.WriteLine("Test data inserted.");
        }

        public IDatabase GetDatabaseInstance()
        {
            return new SqlServerDatabase(ConnectionString);
        }
    }
}