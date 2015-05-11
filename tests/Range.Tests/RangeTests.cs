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
            InlineData("(123,)")]
            public void Can_parse_valid_string(string value)
            {
                var @return = Range.FromString<int>(value);

                Assert.NotNull(@return);
            }

            [Theory,
            InlineData("(123,456)", false),
            InlineData("[123,456)", true)]
            public void Properly_sets_minimum_inclusive_flag(
                string parseValue,
                bool expectedValue)
            {
                var @return = Range.FromString<int>(parseValue);

                Assert.Equal(expectedValue, @return.IsMinInclusive);
            }

            [Theory,
            InlineData("(123,456)", false),
            InlineData("(123,456]", true)]
            public void Properly_sets_maximum_inclusive_flag(
                string parseValue,
                bool expectedValue)
            {
                var @return = Range.FromString<int>(parseValue);

                Assert.Equal(expectedValue, @return.IsMaxInclusive);
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
            InlineData("<123,456>"),
            InlineData("{123,456)"),
            InlineData("(123,456}")]
            public void Throws_if_format_is_not_valid(string value)
            {
                var exception = Assert.Throws<FormatException>(() =>
                {
                    Range.FromString<int>(value);
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    "value does not match expected format.",
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
        }
    }
}
