using System;
using System.Linq;
using System.Collections.Generic;
using RimDev.Filter.Range.Generic;
using RimDev.Filter.Generic;
using Xunit;

namespace RimDev.Filter.Tests.Generic
{
    public class IEnumerable_1ExtensionsTests
    {
        public class Filter : IEnumerable_1ExtensionsTests
        {
            public class Person
            {
                public DateTime FavoriteDate { get; set; }
                public char FavoriteLetter { get; set; }
                public int FavoriteNumber { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public decimal? Rating { get; set; }
                public bool? IsGreat { get; set; }
            }

            protected readonly IEnumerable<Person> People = new List<Person>()
            {
                new Person()
                {
                    FavoriteDate = DateTime.Parse("2000-01-01"),
                    FavoriteLetter = 'a',
                    FavoriteNumber = 5,
                    FirstName = "John",
                    LastName = "Doe",
                    IsGreat = null
                },
                new Person()
                {
                    FavoriteDate = DateTime.Parse("2000-01-02"),
                    FavoriteLetter = 'b',
                    FavoriteNumber = 10,
                    FirstName = "Tim",
                    LastName = "Smith",
                    Rating = 4.5m,
                    IsGreat = true
                },
            };
        }

        public class ConstantFilters : Filter
        {
            [Fact]
            public void Should_filter_when_property_types_match_as_constant_string()
            {
                var @return = People.Filter(new
                {
                    FirstName = "John"
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_constant_integer()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = 5
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_multiple_properties()
            {
                var @return = People.Filter(new
                {
                    FirstName = "John",
                    FavoriteNumber = 0
                });

                Assert.NotNull(@return);
                Assert.Equal(0, @return.Count());
            }

            [Fact]
            public void Should_not_throw_if_filter_does_not_contain_valid_properties()
            {
                var @return = People.Filter(new
                {
                    DOESNOTEXIST = ""
                });

                Assert.NotNull(@return);
                Assert.Equal(2, @return.Count());
            }

            [Fact]
            public void Should_not_throw_if_filter_constant_type_does_not_match()
            {
                var @return = People.Filter(new
                {
                    FirstName = 1
                });

                Assert.NotNull(@return);
                Assert.Equal(2, @return.Count());
            }

            [Fact]
            public void Should_filter_on_nullable_booleans()
            {
                var @return = People.Filter(new
                {
                    IsGreat = true
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("Tim", @return.First().FirstName);
            }
        }

        public class EnumerableFilters : Filter
        {
            [Fact]
            public void Should_bypass_filter_when_empty_collection()
            {
                var @return = People.Filter(new
                {
                    FirstName = new string[] { }
                });

                Assert.NotNull(@return);
                Assert.Equal(2, @return.Count());
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_enumerable_string()
            {
                var @return = People.Filter(new
                {
                    FirstName = new[] { "John" }
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_enumerable_integer()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = new[] { 5 }
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_be_able_to_handle_nullable_destination_and_primitive_filter()
            {
                var @return = People.Filter(new
                {
                    Rating = new[] { 4.5m }
                });

                Assert.Equal(1, @return.Count());
            }

            [Fact]
            public void Should_be_able_to_handle_nullable_destination_and_nullable_primitive_filter_as_collection()
            {
                var @return = People.Filter(new
                {
                    Rating = new decimal?[] { 4.5m }
                });

                Assert.Equal(1, @return.Count());
            }

            [Fact]
            public void Should_be_able_to_handle_nullable_destination_and_nullable_primitive_filter()
            {
                var @return = People.Filter(new
                {
                    Rating = new decimal?(4.5m)
                });

                Assert.Equal(1, @return.Count());
            }

            [Fact]
            public void Should_be_able_to_handle_nullable_source()
            {
                var people = People.ToList();
                people.ForEach(x => x.Rating = null);

                var @return = people.Filter(new
                {
                    Rating = new decimal?[] { 4.5m }
                });

                Assert.Equal(0, @return.Count());
            }
        }

        public class RangeFilters : Filter
        {
            [Theory,
            InlineData("(,5]"),
            InlineData("(-∞,5]")]
            public void Should_filter_open_ended_lower_bound(string value)
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<int>(value)
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Theory,
            InlineData("(5,]"),
            InlineData("(5,+∞)")]
            public void Should_filter_open_ended_upper_bound(string value)
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<int>(value)
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("Tim", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_for_concrete_range()
            {
                var @return = People.Filter(new
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
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_byte()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<byte>("[5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_char()
            {
                var @return = People.Filter(new
                {
                    FavoriteLetter = Range.Range.FromString<char>("[a,b)")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_datetime()
            {
                var @return = People.Filter(new
                {
                    FavoriteDate = Range.Range.FromString<DateTime>("[2000-01-01,2000-01-02)")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_decimal()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<decimal>("[4.5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_double()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<double>("[4.5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_float()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<float>("[4.5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_int()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<int>("[5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_long()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<long>("[5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_sbyte()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<sbyte>("[5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_short()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<short>("[5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_uint()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<uint>("[5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_ulong()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<ulong>("[5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_ushort()
            {
                var @return = People.Filter(new
                {
                    FavoriteNumber = Range.Range.FromString<ushort>("[5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_be_able_to_handle_nullable_source()
            {
                var @return = People.Filter(new
                {
                    Rating = (Range<decimal>)"[4.5,5.0]"
                });

                Assert.Equal(1, @return.Count());
            }
        }
    }
}
