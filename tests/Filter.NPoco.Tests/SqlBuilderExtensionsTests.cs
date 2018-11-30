using NPoco;
using RimDev.Automation.Sql;
using RimDev.Filter.NPoco;
using RimDev.Filter.Range.Generic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace RimDev.Filter.Tests
{
    public class SqlBuilderExtensionsTests
    {
        public class Filter : SqlBuilderExtensionsTests, IDisposable
        {
            public Filter()
            {
                localDb = new LocalDb(version: "v13.0");

                using (var connection = new SqlConnection(localDb.ConnectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"
create table Person(
    Id int identity not null,
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

            [TableName("Person")]
            [PrimaryKey(nameof(Id))]
            public class Person
            {
                public int Id { get; set; }
                public DateTime FavoriteDate { get; set; }
                public DateTimeOffset FavoriteDateTimeOffset { get; set; }
                public char FavoriteLetter { get; set; }
                public int FavoriteNumber { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public decimal? Rating { get; set; }
            }

            protected readonly IDatabase Database;

            private readonly IEnumerable<Person> people = new List<Person>()
            {
                new Person()
                {
                    FavoriteDate = DateTime.Parse("2000-01-01"),
                    FavoriteDateTimeOffset = DateTimeOffset.Parse("2010-01-01"),
                    FavoriteLetter = 'a',
                    FavoriteNumber = 5,
                    FirstName = "John",
                    LastName = "Doe"
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

        public class ConstantFilters : Filter
        {
            [Fact]
            public void Should_filter_when_property_types_match_as_constant_string()
            {
                var @return = Database
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
                var @return = Database
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
                var @return = Database
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
                var @return = Database
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
                var @return = Database
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
            [Fact]
            public void Should_bypass_filter_when_empty_collection()
            {
                var @return = Database
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
                var @return = Database
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
                var @return = Database
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
                var @return = Database
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
                var @return = Database
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
                var @return = Database
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
                var people = Database.Query<Person>().ToList();
                people.ForEach(x => x.Rating = null);

                foreach (var person in people)
                {
                    Database.Update(person);
                }

                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        Rating = new decimal?[] { 4.5m }
                    });

                Assert.Empty(@return);
            }
        }

        public class RangeFilters : Filter
        {
            [Theory,
            InlineData("(,5]"),
            InlineData("(-∞,5]")]
            public void Should_filter_open_ended_lower_bound(string value)
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<int>(value)
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
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<int>(value)
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("Tim", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_for_concrete_range()
            {
                var @return = Database
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
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<byte>("[5,5]")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_char()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteLetter = Range.Range.FromString<char>("[a,b)")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_datetime()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteDate = Range.Range.FromString<DateTime>("[2000-01-01,2000-01-02)")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_datetimeoffset()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteDateTimeOffset = Range.Range.FromString<DateTimeOffset>("[2010-01-01,2010-01-02)")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_decimal()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<decimal>("[4.5,5]")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_double()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<double>("[4.5,5]")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_float()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<float>("[4.5,5]")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_int()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<int>("[5,5]")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_long()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<long>("[5,5]")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_sbyte()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<sbyte>("[5,5]")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_short()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<short>("[5,5]")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_uint()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<uint>("[5,5]")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_ulong()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<ulong>("[5,5]")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_filter_when_property_types_match_as_range_ushort()
            {
                var @return = Database
                    .With<Person>("where /**where**/")
                    .Fetch(new
                    {
                        FavoriteNumber = Range.Range.FromString<ushort>("[5,5]")
                    });

                Assert.NotNull(@return);
                Assert.Single(@return);
                Assert.Equal("John", @return.First().FirstName);
            }

            [Fact]
            public void Should_be_able_to_handle_nullable_source()
            {
                var @return = Database
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
