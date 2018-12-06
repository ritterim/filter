using RimDev.Filter.NPoco;
using RimDev.Filter.Range;
using RimDev.Filter.Range.Generic;
using System;
using System.Linq;
using Xunit;

namespace Filter.NPoco.Tests
{
    public class SqlBuilderExtensionsTests
    {
        public abstract class Filter : SqlBuilderExtensionsTests, IClassFixture<DatabaseFixture>
        {
            public Filter(DatabaseFixture fixture)
            {
                Fixture = fixture;
            }

            protected readonly DatabaseFixture Fixture;
        }

        public class ConstantFilters : Filter
        {
            public ConstantFilters(DatabaseFixture fixture)
                : base(fixture)
            { }

            [Fact]
            public void Should_filter_when_property_types_match_as_constant_string()
            {
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FirstName = 1
                    });

                Assert.NotNull(@return);
                Assert.Equal(2, @return.Count());
            }
        }

        public class EnumerableFilters : Filter
        {
            public EnumerableFilters(DatabaseFixture fixture)
                : base(fixture)
            { }

            [Fact]
            public void Should_bypass_filter_when_empty_collection()
            {
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        DONOTUSE = new string[] { "whatever" }
                    });

                Assert.Empty(@return);
            }
        }

        public class RangeFilters : Filter
        {
            public RangeFilters(DatabaseFixture fixture)
                : base(fixture)
            { }

            [Theory,
            InlineData("(,5]"),
            InlineData("(-∞,5]")]
            public void Should_filter_open_ended_lower_bound(string value)
            {
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
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
                var @return = Fixture.Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        Rating = (Range<decimal>)"[4.5,5.0]"
                    });

                Assert.Single(@return);
            }
        }
    }
}
