using RimDev.Filter.Range.Generic;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RimDev.Filter.Range
{
    public static class Range
    {
        public static IRange<T> FromString<T>(string value)
            where T : struct
        {
            var range = new Range<T>();

            /**
             * This expression is very cumbersome due to its requirement of handling
             * several formats. Please take note of the lookbehind assertion and its preceeding
             * word-boundary. The former lets us define a max-value using simple-syntax while
             * the latter accounts for the min-value (due to the assertion).
             * Valid formats (not exhaustive):
             * [123,456]
             * [123,
             * [123,456
             * 123
             * 123,456
             * 123,
             */
            var parsedValue = Regex.Match(
                value + ",",
                @"(?<startSyntax>[\[\(])?(?<minValue>[^,[(]+?)?(?:[\W])(?<=,)(?<maxValue>[^,\]\)]+)?(?<endSyntax>[\]\)])?");

            if (!parsedValue.Success)
            {
                throw new FormatException("value does not match expected format.");
            }
            else
            {
                var groups = parsedValue.Groups;
                var parsedMinValue = groups["minValue"].Value;

                /**
                 * If we using short-range syntax, then we
                 * want to set the max-value if not provided.
                 */
                var parsedMaxValue =
                    string.IsNullOrEmpty(
                    groups["maxValue"].Value + groups["startSyntax"].Value + groups["endSyntax"].Value)
                    ? parsedMinValue
                    : groups["maxValue"].Value;

                var isMinInclusive =
                    (groups["startSyntax"].Value == "[" || string.IsNullOrEmpty(groups["startSyntax"].Value))
                    ? true
                    : false;
                var isMaxInclusive =
                    (groups["endSyntax"].Value == "]" || string.IsNullOrEmpty(groups["endSyntax"].Value))
                    ? true
                    : false;
                var isMinInfinite = parsedMinValue == "-∞" ? true : false;
                var isMaxInfinite = parsedMaxValue == "+∞" ? true : false;

                if (string.IsNullOrWhiteSpace(parsedMinValue) &&
                    string.IsNullOrWhiteSpace(parsedMaxValue))
                {
                    throw new FormatException("value cannot be open-ended for both min and max-values.");
                }

                if (isMinInclusive && isMinInfinite)
                {
                    throw new FormatException("value cannot have inclusive infinite lower-bound.");
                }

                if (isMaxInclusive && isMaxInfinite)
                {
                    throw new FormatException("value cannot have inclusive infinite upper-bound.");
                }

                T? minValue = default(T?);
                T? maxValue = default(T?);

                if (!string.IsNullOrWhiteSpace(parsedMinValue) && !isMinInfinite)
                {
                    try
                    {
                        minValue = (T)Convert.ChangeType(parsedMinValue, typeof(T));
                    }
                    catch (Exception)
                    {
                        throw new FormatException(
                            string.Format("parsed minimum value `{0}` does not match expected type of `{1}`.",
                            groups["minValue"].Value,
                            typeof(T).Name));
                    }
                }

                if (!string.IsNullOrWhiteSpace(parsedMaxValue) && !isMaxInfinite)
                {
                    try
                    {
                        maxValue = (T)Convert.ChangeType(parsedMaxValue, typeof(T));
                    }
                    catch (Exception)
                    {
                        throw new FormatException(
                            string.Format("parsed maximum value `{0}` does not match expected type of `{1}`.",
                            groups["maxValue"].Value,
                            typeof(T).Name));
                    }
                }

                range.MinValue = minValue;
                range.MaxValue = maxValue;
                range.IsMinInclusive = isMinInclusive;
                range.IsMaxInclusive = isMaxInclusive;
            }

            return range;
        }
    }
}
