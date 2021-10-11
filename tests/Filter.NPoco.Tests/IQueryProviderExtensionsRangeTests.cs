using RimDev.Filter.NPoco;
using RimDev.Filter.Range.Generic;
using System;
using Filter.NPoco.Tests.Testing;
using Xunit;
using Range = RimDev.Filter.Range.Range;

namespace Filter.NPoco.Tests
{
    [Collection(nameof(NPocoDatabaseCollection))]
    public class IQueryProviderExtensionsRangeTests
    {
        private readonly NPocoDatabaseFixture fixture;

        public IQueryProviderExtensionsRangeTests(NPocoDatabaseFixture fixture)
        {
            this.fixture = fixture;
        }
        
        [Theory,
        InlineData("(,5]"),
        InlineData("(-∞,5]")]
        public void Should_filter_open_ended_lower_bound(string value)
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<int>(value)
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Theory,
        InlineData("(5,]"),
        InlineData("(5,+∞)")]
        public void Should_filter_open_ended_upper_bound(string value)
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<int>(value)
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("Tim", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_for_concrete_range()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = new Range<int>()
                {
                    MinValue = 5,
                    MaxValue = 5,
                    IsMinInclusive = true,
                    IsMaxInclusive = true
                }
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_byte()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<byte>("[5,5]")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_char()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteLetter = Range.FromString<char>("[a,b)")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_datetime()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteDate = Range.FromString<DateTime>("[2000-01-01,2000-01-02)")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_datetimeoffset()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteDateTimeOffset = Range.FromString<DateTimeOffset>("[2010-01-01,2010-01-02)")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_decimal()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<decimal>("[4.5,5]")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_double()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<double>("[4.5,5]")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_float()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<float>("[4.5,5]")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_int()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<int>("[5,5]")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_long()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<long>("[5,5]")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_sbyte()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<sbyte>("[5,5]")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_short()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<short>("[5,5]")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_uint()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<uint>("[5,5]")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_ulong()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<ulong>("[5,5]")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_filter_when_property_types_match_as_range_ushort()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                FavoriteNumber = Range.FromString<ushort>("[5,5]")
            });

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            Assert.Equal("John", results.First().FirstName);
        }

        [Fact]
        public void Should_be_able_to_handle_nullable_source()
        {
            var results = fixture.GetDatabaseInstance().Query<Person>().Filter(new
            {
                Rating = (Range<decimal>)"[4.5,5.0]"
            });

            Assert.Equal(1, results.Count());
        }
    }
}
