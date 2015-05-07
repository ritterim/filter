using RimDev.Filter.Range.Generic;
using System;
using System.Text.RegularExpressions;

namespace RimDev.Filter.Range
{
    public static class Range
    {
        public static IRange<T> FromString<T>(string value)
        {
            var range = new Range<T>();

            var parsedValue = Regex.Match(value, "([\\[\\(])(.+?),(.+?)([\\]\\)])");

            if (!parsedValue.Success)
            {
                throw new FormatException("value does not match expected format.");
            }
            else
            {
                var groups = parsedValue.Groups;

                T minValue = default(T);
                T maxValue = default(T);

                try
                {
                    minValue = (T)Convert.ChangeType(groups[2].Value, typeof(T));
                }
                catch (Exception)
                {
                    throw new FormatException(
                        string.Format("parsed minimum value `{0}` does not match expected type of `{1}`.",
                        groups[2].Value,
                        typeof(T).Name));
                }

                try
                {
                    maxValue = (T)Convert.ChangeType(groups[3].Value, typeof(T));
                }
                catch (Exception)
                {
                    throw new FormatException(
                        string.Format("parsed maximum value `{0}` does not match expected type of `{1}`.",
                        groups[3].Value,
                        typeof(T).Name));
                }

                range.MinValue = minValue;
                range.MaxValue = maxValue;
                range.IsMinInclusive = groups[1].Value == "[" ? true : false;
                range.IsMaxInclusive = groups[4].Value == "]" ? true : false;
            }

            return range;
        }
    }
}
