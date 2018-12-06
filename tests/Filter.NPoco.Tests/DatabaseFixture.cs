using NPoco;
using RimDev.Automation.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Filter.NPoco.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            string dbSuffix = string.Empty;

            do
            {
                dbSuffix = Guid.NewGuid().ToString("N");
            } while (dbSuffixes.Contains(dbSuffix));

            dbSuffixes.Add(dbSuffix);

            localDb = new LocalDb(version: "v13.0", databaseSuffixGenerator: () => dbSuffix);

            using (var connection = new SqlConnection(localDb.ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
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
            }

            Database = new Database(localDb.ConnectionString, DatabaseType.SqlServer2008);

            Database.InsertBulk(people);
        }

        public readonly IDatabase Database;

        private static List<string> dbSuffixes = new List<string>();

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

        private LocalDb localDb;

        public void Dispose()
        {
            localDb.Dispose();
            Database.Dispose();
        }
    }
}
