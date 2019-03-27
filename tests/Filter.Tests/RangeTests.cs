using RimDev.Filter.Range.Generic;
using System;
using Xunit;

namespace RimDev.Filter.Range.Tests
{
    public class RangeTests
    {
        public class FromString
        {
            [Theory,
            InlineData("(-∞,456)"),
            InlineData("(123,+∞)")]
            public void Can_parse_infinity(string value)
            {
                var @return = Range.FromString<int>(value);

                Assert.NotNull(@return);
            }

            [Theory,
            InlineData("(123,456)"),
            InlineData("(,456)"),
            InlineData("(123,)"),
            InlineData("123"),
            InlineData("123,456")]
            public void Can_parse_valid_string(string value)
            {
                var @return = Range.FromString<int>(value);

                Assert.NotNull(@return);
            }

            [Theory,
            InlineData("[2017-01-01T00:00:00.000+00:00, 2018-02-02T00:00:00.000+00:00)")]
            public void Can_parse_valid_date_string_as_datetime(string value)
            {
                var @return = Range.FromString<DateTime>(value);

                Assert.NotNull(@return);
            }

            [Theory,
            InlineData("[2017-01-01T00:00:00.000+00:00, 2018-02-02T00:00:00.000+00:00)")]
            public void Can_parse_valid_date_string_as_datetimeoffset(string value)
            {
                var @return = Range.FromString<DateTimeOffset>(value);

                Assert.NotNull(@return);
            }

            [Theory,
            InlineData("[2017-01-01,2018-02-02)")]
            public void Can_parse_valid_simple_date_string_as_datetimeoffset(string value)
            {
                var @return = Range.FromString<DateTimeOffset>(value);

                Assert.NotNull(@return);
            }

            [Theory,
            InlineData("(123,456)", false),
            InlineData("[123,456)", true),
            InlineData("123", true),
            InlineData("(123", false),
            InlineData("[123", true),
            InlineData("(123,456", false),
            InlineData("[123,456", true)]
            public void Properly_sets_minimum_inclusive_flag(
                string parseValue,
                bool expectedValue)
            {
                var @return = Range.FromString<int>(parseValue);

                Assert.Equal(expectedValue, @return.IsMinInclusive);
            }

            [Theory,
            InlineData("(123,456)", false),
            InlineData("(123,456]", true),
            InlineData("123", true),
            InlineData("(123,", true),
            InlineData("[123", true),
            InlineData("123,456", true),
            InlineData("123,456]", true),
            InlineData("123,456)", false)]
            public void Properly_sets_maximum_inclusive_flag(
                string parseValue,
                bool expectedValue)
            {
                var @return = Range.FromString<int>(parseValue);

                Assert.Equal(expectedValue, @return.IsMaxInclusive);
            }

            [Theory,
            InlineData("[123", null),
            InlineData("123,456)", 456)]
            public void Properly_sets_maximum_value_if_not_using_short_syntax(
                string value,
                int? expectedMaxValue)
            {
                var @return = Range.FromString<int>(value);

                Assert.Equal(expectedMaxValue, @return.MaxValue);
            }

            [Theory,
            InlineData("123", 123),
            InlineData("123,456", 456),
            InlineData("123,", 123)]
            public void Properly_sets_maximum_value_if_using_short_syntax(
                string value,
                int expectedMaxValue)
            {
                var @return = Range.FromString<int>(value);

                Assert.Equal(expectedMaxValue, @return.MaxValue);
            }

            [Theory,
            InlineData("123", 123),
            InlineData(",123", null)]
            public void Properly_sets_minimum_value_if_using_short_syntax(
                string value,
                int? expectedMinValue)
            {
                var @return = Range.FromString<int>(value);

                Assert.Equal(expectedMinValue, @return.MinValue);
            }

            [Fact]
            public void Throws_if_both_min_and_max_are_open_ended()
            {
                var exception = Assert.Throws<FormatException>(() =>
                {
                    Range.FromString<int>("(,)");
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    "value cannot be open-ended for both min and max-values.",
                    exception.Message);
            }

            [Theory,
            InlineData("<123,456>", "parsed minimum value `<123` does not match expected type of `Int32`."),
            InlineData("{123,456)", "parsed minimum value `{123` does not match expected type of `Int32`."),
            InlineData("(123,456}", "parsed maximum value `456}` does not match expected type of `Int32`.")]
            public void Throws_if_format_is_not_valid(string value, string expectedMessage)
            {
                var exception = Assert.Throws<FormatException>(() =>
                {
                    Range.FromString<int>(value);
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    expectedMessage,
                    exception.Message);
            }

            [Fact]
            public void Throws_if_inclusive_infinite_lower_bound()
            {
                var exception = Assert.Throws<FormatException>(() =>
                {
                    Range.FromString<int>("[-∞,456)");
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    "value cannot have inclusive infinite lower-bound.",
                    exception.Message);
            }

            [Fact]
            public void Throws_if_inclusive_infinite_upper_bound()
            {
                var exception = Assert.Throws<FormatException>(() =>
                {
                    Range.FromString<int>("(123,+∞]");
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    "value cannot have inclusive infinite upper-bound.",
                    exception.Message);
            }

            [Fact]
            public void Throws_if_parsed_minimum_value_does_not_match_expected_type()
            {
                var exception = Assert.Throws<FormatException>(() =>
                {
                    Range.FromString<int>("(abc,123)");
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    "parsed minimum value `abc` does not match expected type of `Int32`.",
                    exception.Message);
            }

            [Fact]
            public void Throws_if_parsed_maximum_value_does_not_match_expected_type()
            {
                var exception = Assert.Throws<FormatException>(() =>
                {
                    Range.FromString<int>("(123,abc)");
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    "parsed maximum value `abc` does not match expected type of `Int32`.",
                    exception.Message);
            }

            [Fact]
            public void Validate_throws_if_maximum_is_less_than_minimum_with_int()
            {
                var rangeResult = Range.FromString<int>("[2,1]");

                var exception = Assert.Throws<InvalidOperationException>(() =>
                {
                    rangeResult.Validate();
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    "Minimum value of range must be less than or equal to maximum value.",
                    exception.Message);
            }

            [Theory,
            InlineData("[2017-01-01T00:00:00.000+00:00, 2016-01-01T00:00:00.000+00:00]")]
            public void Validate_throws_if_maximum_is_less_than_minimum_with_datetime(string value)
            {
                var rangeResult = Range.FromString<DateTime>(value);

                var exception = Assert.Throws<InvalidOperationException>(() =>
                {
                    rangeResult.Validate();
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    "Minimum value of range must be less than or equal to maximum value.",
                    exception.Message);
            }

            [Theory,
            InlineData("[2017-01-01T00:00:00.000+00:00, 2016-01-01T00:00:00.000+00:00]")]
            public void Validate_throws_if_maximum_is_less_than_minimum_with_datetimeoffset(string value)
            {
                var rangeResult = Range.FromString<DateTimeOffset>(value);

                var exception = Assert.Throws<InvalidOperationException>(() =>
                {
                    rangeResult.Validate();
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    "Minimum value of range must be less than or equal to maximum value.",
                    exception.Message);
            }

            [Fact]
            public void Validate_throws_if_maximum_is_equal_to_minimum_when_non_inclusive()
            {
                var rangeResult = Range.FromString<int>("(2,2)");

                var exception = Assert.Throws<InvalidOperationException>(() =>
                {
                    rangeResult.Validate();
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    "Minimum value of range must be less than maximum value when range is non-inclusive.",
                    exception.Message);
            }

            [Fact]
            public void Implicitly_cast_from_string_to_range()
            {
                Range<int> range = "[1,5]";

                Assert.NotNull(range);
                Assert.Equal(1, range.MinValue);
                Assert.True(range.IsMinInclusive);
                Assert.True(range.IsMaxInclusive);
                Assert.Equal(5, range.MaxValue);
            }

            public class ImplicitOperatorStringTests : RangeTests
            {
                [Theory,
                InlineData("[1,5]"),
                InlineData("1,5")]
                public void Implicitly_cast_from_range_to_string(string value)
                {
                    var range = Range.FromString<int>(value) as Range<int>;
                    string actualValue = range;

                    Assert.Equal("[1,5]", actualValue);
                }
            }

            public class ImplicitOperatorRangeTests : RangeTests
            {
                [Theory,
                InlineData("[1,5]"),
                InlineData("1,5")]
                public void Directly_cast_from_a_string(string value)
                {
                    var range = (Range<int>)value;

                    Assert.NotNull(range);
                    Assert.Equal(1, range.MinValue);
                    Assert.True(range.IsMinInclusive);
                    Assert.True(range.IsMaxInclusive);
                    Assert.Equal(5, range.MaxValue);
                }
            }
        }
    }
}
