using RimDev.Automation.Sql;
using RimDev.Filter.Generic;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Xunit;

namespace RimDev.Filter.Tests.Generic.Integration
{
    public class EfTests
    {
        private readonly IEnumerable<Person> People = new List<Person>()
        {
            new Person()
            {
                FavoriteDate = DateTime.Parse("2000-01-01"),
                FavoriteLetter = 'a',
                FavoriteNumber = 5,
                FirstName = "John",
                LastName = "Doe"
            },
            new Person()
            {
                FavoriteDate = DateTime.Parse("2000-01-02"),
                FavoriteLetter = 'b',
                FavoriteNumber = 10,
                FirstName = "Tim",
                LastName = "Smith",
                Rating = 4.5m
            },
        };

        [Fact]
        public void Can_filter_nullable_models_via_entity_framework()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<FilterDbContext>());

            using (var database = new LocalDb())
            {
                using (var context = new FilterDbContext(database.ConnectionString))
                {
                    context.People.AddRange(People);
                    context.SaveChanges();

                    var @return = People.Filter(new
                    {
                        Rating = new decimal?(4.5m)
                    });

                    Assert.Equal(1, @return.Count());
                }
            }
        }

        public class FilterDbContext : DbContext
        {
            public FilterDbContext(string nameOrConnectionString)
                : base(nameOrConnectionString)
            { }

            public DbSet<Person> People
            {
                get; set;
            }
        }

        public class Person
        {
            public int Id { get; set; }
            public DateTime FavoriteDate { get; set; }
            public char FavoriteLetter { get; set; }
            public int FavoriteNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public decimal? Rating { get; set; }
        }
    }
}
