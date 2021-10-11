using RimDev.Filter.Range.Generic;
using System;
using System.Linq;
using Filter.NPoco.Tests.Testing;
using RimDev.Filter.NPoco;
using Xunit;
using Range = RimDev.Filter.Range.Range;

namespace Filter.NPoco.Tests
{
    [Collection(nameof(NPocoDatabaseCollection))]
    public class SqlBuilderExtensionsRangeTests
    {
        private readonly NPocoDatabaseFixture fixture;

        public SqlBuilderExtensionsRangeTests(NPocoDatabaseFixture fixture)
        {
            this.fixture = fixture;
        }
        
        [Theory,
        InlineData("(,5]"),
        InlineData("(-∞,5]")]
        public void Should_filter_open_ended_lower_bound(string value)
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<int>(value)
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Theory,
        InlineData("(5,]"),
        InlineData("(5,+∞)")]
        public void Should_filter_open_ended_upper_bound(string value)
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<int>(value)
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("Tim", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_for_concrete_range()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = new Range<int>()
                    {
                        MinValue = 5,
                        MaxValue = 5,
                        IsMinInclusive = true,
                        IsMaxInclusive = true
                    }
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_byte()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<byte>("[5,5]")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_char()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteLetter = Range.FromString<char>("[a,b)")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_datetime()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteDate = Range.FromString<DateTime>("[2000-01-01,2000-01-02)")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_datetimeoffset()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteDateTimeOffset = Range.FromString<DateTimeOffset>("[2010-01-01,2010-01-02)")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_decimal()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<decimal>("[4.5,5]")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_double()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<double>("[4.5,5]")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_float()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<float>("[4.5,5]")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_int()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<int>("[5,5]")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_long()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<long>("[5,5]")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_sbyte()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<sbyte>("[5,5]")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_short()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<short>("[5,5]")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_uint()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<uint>("[5,5]")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_ulong()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<ulong>("[5,5]")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_ushort()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    FavoriteNumber = Range.FromString<ushort>("[5,5]")
                });

            Assert.NotNull(@return);
            Assert.Single(@return);
            Assert.Equal("John", @return.First().FirstName);
        }

        [Fact]
        public void Should_be_able_to_handle_nullable_source()
        {
            var @return = fixture.GetDatabaseInstance()
                .With<Person>("where /**where**/")
                .Fetch(new
                {
                    Rating = (Range<decimal>)"[4.5,5.0]"
                });

            Assert.Single(@return);
        }
    }
}
