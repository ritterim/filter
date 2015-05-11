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

            var parsedValue = Regex.Match(value, "([\\[\\(])(.+?)?,(.+?)?([\\]\\)])");

            if (!parsedValue.Success)
            {
                throw new FormatException("value does not match expected format.");
            }
            else
            {
                var groups = parsedValue.Groups;
                var parsedMinValue = groups[2].Value;
                var parsedMaxValue = groups[3].Value;
                var isMinInclusive = groups[1].Value == "[" ? true : false;
                var isMaxInclusive = groups[4].Value == "]" ? true : false;
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
                            groups[2].Value,
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
                            groups[3].Value,
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
