using RimDev.Filter.NPoco;
using Filter.NPoco.Tests.Testing;
using Xunit;

namespace Filter.NPoco.Tests
{
    [Collection(nameof(NPocoDatabaseCollection))]
    public class IQueryProviderExtensionsTests
    {
        private readonly NPocoDatabaseFixture fixture;

        public IQueryProviderExtensionsTests(NPocoDatabaseFixture fixture)
        {
            this.fixture = fixture;
        }
        
        [Fact]
        public void Should_filter_when_property_types_match_as_constant_string()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    FirstName = "John"
                });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_constant_integer()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    FavoriteNumber = 5
                });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_multiple_properties()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    FirstName = "John",
                    FavoriteNumber = 0
                });

            Assert.NotNull(results);
            Assert.Equal(0, results.Count());
        }

        [Fact]
        public void Should_not_throw_if_filter_does_not_contain_valid_properties()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    DOESNOTEXIST = ""
                });

            Assert.NotNull(results);
            Assert.Equal(2, results.Count());
        }

        [Fact]
        public void Should_not_throw_if_filter_constant_type_does_not_match()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    FirstName = 1
                });

            Assert.NotNull(results);
            Assert.Equal(2, results.Count());
        }

        
        [Fact]
        public void Should_bypass_filter_when_empty_collection()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    FirstName = new string[] { }
                });

            Assert.NotNull(results);
            Assert.Equal(2, results.Count());
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_enumerable_string()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    FirstName = new[] { "John" }
                });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_enumerable_integer()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    FavoriteNumber = new[] { 5 }
                });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_be_able_to_handle_nullable_destination_and_primitive_filter()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    Rating = new[] { 4.5m }
                });

            Assert.Equal(1, results.Count());
        }

        [Fact]
        public void Should_be_able_to_handle_nullable_destination_and_nullable_primitive_filter_as_collection()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    Rating = new decimal?[] { 4.5m }
                });

            Assert.Equal(1, results.Count());
        }

        [Fact]
        public void Should_be_able_to_handle_nullable_destination_and_nullable_primitive_filter()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    Rating = new decimal?(4.5m)
                });

            Assert.Equal(1, results.Count());
        }

        [Fact]
        public void Should_be_able_to_handle_nullable_source()
        {
            var results = fixture.GetDatabaseInstance()
                .Query<Person>()
                .Filter(new
                {
                    DONOTUSE = new string[] { "whatever" }
                }).ToList();

            Assert.Empty(results);
        }
    }
}
