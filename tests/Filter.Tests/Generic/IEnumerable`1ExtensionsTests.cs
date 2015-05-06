using RimDev.Filter.Generic;
using RimDev.Filter.Range;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Filter.Tests.Generic
{
    public class IEnumerable_1ExtensionsTests
    {
        public class Filter : IEnumerable_1ExtensionsTests
        {
            private class Person
            {
                public int FavoriteNumber { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
            }

            private readonly IEnumerable<Person> people = new List<Person>()
            {
                new Person()
                {
                    FavoriteNumber = 5,
                    FirstName = "John",
                    LastName = "Doe"
                },
                new Person()
                {
                    FavoriteNumber = 10,
                    FirstName = "Tim",
                    LastName = "Smith"
                },
            };

            [Fact]
            public void Should_filter_when_property_types_match_as_constant_string()
            {
                var @return = people.Filter(new
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
                var @return = people.Filter(new
                {
                    FavoriteNumber = 5
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_enumerable_string()
            {
                var @return = people.Filter(new
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
                var @return = people.Filter(new
                {
                    FavoriteNumber = new[] { 5 }
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_int()
            {
                var @return = people.Filter(new
                {
                    FavoriteNumber = Range.FromString<int>("[5,5]")
                });

                Assert.NotNull(@return);
                Assert.Equal(1, @return.Count());
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_multiple_properties()
            {
                var @return = people.Filter(new
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
                var @return = people.Filter(new
                {
                    DOESNOTEXIST = ""
                });

                Assert.NotNull(@return);
                Assert.Equal(2, @return.Count());
            }

            [Fact]
            public void Should_not_throw_if_filter_constant_type_does_not_match()
            {
                var @return = people.Filter(new
                {
                    FirstName = 1
                });

                Assert.NotNull(@return);
                Assert.Equal(2, @return.Count());
            }
        }
    }
}
