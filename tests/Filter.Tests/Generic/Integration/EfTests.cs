using System;
using System.Collections.Generic;
using System.Linq;
using Filter.Tests.Testing;
using RimDev.Filter.Generic;
using RimDev.Filter.Range.Generic;
using Xunit;

namespace Filter.Tests.Generic.Integration
{
    [Collection(nameof(EntityFrameworkDatabaseCollection))]
    public class EfTests
    {
        private readonly EntityFrameworkDatabaseFixture fixture;

        public EfTests(EntityFrameworkDatabaseFixture entityFrameworkDatabaseFixture)
        {
            fixture = entityFrameworkDatabaseFixture;
        }

        [Fact]
        public void Can_filter_nullable_models_via_entity_framework()
        {
            using var context = new TestDbContext(fixture.ConnectionString);
            var results = context.People.Filter(new
            {
                Rating = new decimal?(4.5m)
            });

            Assert.Equal(1, results.Count());
        }

        [Fact]
        public void Can_filter_datetimeoffset_via_entity_framework()
        {
            using var context = new TestDbContext(fixture.ConnectionString);

            var results = context.People.Filter(new
            {
                FavoriteDateTimeOffset = (Range<DateTimeOffset>)"[2010-01-01,2010-01-02)"
            });

            Assert.Equal(1, results.Count());
        }

        [Fact]
        public void Should_be_able_to_handle_nullable_source()
        {
            using var context = new TestDbContext(fixture.ConnectionString);

            var results = context.People.Filter(new
            {
                Rating = (Range<decimal>)"[4.5,5.0]"
            });

            Assert.Equal(1, results.Count());
        }

        [Fact]
        public void Should_not_optimize_arrays_containing_multiple_values()
        {
            var singleParameter = new
            {
                FirstName = "Tim"
            };

            var collectionParameter = new
            {
                FirstName = new[] { "Tim", "John" }
            };

            using (var context = new TestDbContext(fixture.ConnectionString))
            {
                IQueryable<Person> query = context.People.AsNoTracking();

                var expectedQuery = query.Filter(singleParameter);
                var actualQuery = query.Filter(collectionParameter);

                Assert.NotEqual(expectedQuery.ToString(), actualQuery.ToString(), StringComparer.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public void Should_optimize_arrays_containing_a_single_value()
        {
            var singleParameter = new
            {
                FirstName = "Tim"
            };

            var collectionParameter = new
            {
                FirstName = new[] { "Tim" }
            };

            using (var context = new TestDbContext(fixture.ConnectionString))
            {
                IQueryable<Person> query = context.People.AsNoTracking();

                var expectedQuery = query.Where(x => x.FirstName == singleParameter.FirstName);
                var actualQuery = query.Filter(collectionParameter);

                Assert.Equal(expectedQuery.ToString(), actualQuery.ToString(), StringComparer.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public void Should_not_optimize_collections_containing_multiple_values()
        {
            var singleParameter = new
            {
                FirstName = "Tim"
            };

            var collectionParameter = new
            {
                FirstName = new List<string> { "Tim", "John" }
            };

            using (var context = new TestDbContext(fixture.ConnectionString))
            {
                IQueryable<Person> query = context.People.AsNoTracking();

                var expectedQuery = query.Filter(singleParameter);
                var actualQuery = query.Filter(collectionParameter);

                Assert.NotEqual(expectedQuery.ToString(), actualQuery.ToString(), StringComparer.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public void Should_optimize_collections_containing_a_single_value()
        {
            var singleParameter = new
            {
                FirstName = "Tim"
            };

            var collectionParameter = new
            {
                FirstName = new List<string> { "Tim" }
            };

            using (var context = new TestDbContext(fixture.ConnectionString))
            {
                IQueryable<Person> query = context.People.AsNoTracking();

                var expectedQuery = query.Filter(singleParameter);
                var actualQuery = query.Filter(collectionParameter);

                Assert.Equal(expectedQuery.ToString(), actualQuery.ToString(), StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
