using System;
using Xunit;

namespace RimDev.Filter.Range.Tests
{
    public class RangeTests
    {
        public class FromString
        {
            [Fact]
            public void Can_parse_valid_string()
            {
                var @return = Range.FromString<int>("(123,456)");

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
            public void Throws_if_format_is_not_valid()
            {
                var exception = Assert.Throws<FormatException>(() =>
                {
                    Range.FromString<int>("<123,456>");
                });

                Assert.NotNull(exception);
                Assert.Equal(
                    "value does not match expected format.",
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
