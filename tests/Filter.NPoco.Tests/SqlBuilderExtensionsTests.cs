using RimDev.Filter.NPoco;
using RimDev.Filter.Range;
using RimDev.Filter.Range.Generic;
using System;
using System.Linq;
using Filter.NPoco.Tests.Testing;
using Xunit;
using Range = RimDev.Filter.Range.Range;

namespace Filter.NPoco.Tests
{
    [Collection(nameof(NPocoDatabaseCollection))]
    public class SqlBuilderExtensionsTests
    {
        private readonly NPocoDatabaseFixture fixture;

        public SqlBuilderExtensionsTests(NPocoDatabaseFixture fixture)
        {
            this.fixture = fixture;
        }
        
        [Fact]
        public void Should_filter_when_property_types_match_as_constant_string()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FirstName = "John"
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_constant_integer()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = 5
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_multiple_properties()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FirstName = "John",
                    FavoriteNumber = 0
                });

            Assert.NotNull(@return);
            Assert.Empty(@return);
        }

        [Fact]
        public void Should_not_throw_if_filter_does_not_contain_valid_properties()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    DOESNOTEXIST = ""
                });

            Assert.NotNull(@return);
            Assert.Equal(2, @return.Count());
        }

        [Fact]
        public void Should_not_throw_if_filter_constant_type_does_not_match()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FirstName = 1
                });

            Assert.NotNull(@return);
            Assert.Equal(2, @return.Count());
        }

        [Fact]
        public void Should_bypass_filter_when_empty_collection()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FirstName = new string[] { }
                });

            Assert.NotNull(@return);
            Assert.Equal(2, @return.Count());
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_enumerable_string()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FirstName = new[] { "John" }
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_enumerable_integer()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = new[] { 5 }
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_be_able_to_handle_nullable_destination_and_primitive_filter()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    Rating = new[] { 4.5m }
                });

            Assert.Single(@return);
        }

        [Fact]
        public void Should_be_able_to_handle_nullable_destination_and_nullable_primitive_filter_as_collection()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    Rating = new decimal?[] { 4.5m }
                });

            Assert.Single(@return);
        }

        [Fact]
        public void Should_be_able_to_handle_nullable_destination_and_nullable_primitive_filter()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    Rating = new decimal?(4.5m)
                });

            Assert.Single(@return);
        }

        [Fact]
        public void Should_be_able_to_handle_nullable_source()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    DONOTUSE = new string[] { "whatever" }
                });

            Assert.Empty(@return);
        }
    }
}
