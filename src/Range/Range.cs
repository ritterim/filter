﻿using RimDev.Filter.Range.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
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
                        minValue = SmartConverter.Convert<T>(parsedMinValue);
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
                        maxValue = SmartConverter.Convert<T>(parsedMaxValue);
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

        public static bool IsDateRange(object range)
        {
            return range is Range<DateTimeOffset> || range is Range<DateTime>;
        }
               
        public static bool IsNumericRange(object range)
        {
            var rangeDefinition = typeof(Range<>);
            var numbers = new[] 
            {
                typeof(sbyte),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(byte),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal)
            };

            if (range == null)
                return false;

            var target = range.GetType();
            var result = numbers
                .Select(type => rangeDefinition.MakeGenericType(type))
                .FirstOrDefault(type => target == type);

            return result != null;
        }

        public static Range<DateTimeOffset> AsDateRange(object range)
        {
            if (range == null)
                return null;

            switch (IsDateRange(range))
            {
                case true when range is Range<DateTime> dt:
                    return new Range<DateTimeOffset> {
                        IsMaxInclusive = dt.IsMaxInclusive,
                        IsMinInclusive = dt.IsMinInclusive,
                        MinValue = dt.MinValue.HasValue ? 
                            new DateTimeOffset(dt.MinValue.Value, TimeSpan.Zero) 
                            : (DateTimeOffset?) null,
                        MaxValue = dt.MaxValue.HasValue ? 
                            new DateTimeOffset(dt.MaxValue.Value, TimeSpan.Zero) 
                            : (DateTimeOffset?) null,
                    };
                case true when range is Range<DateTimeOffset> dto:
                    return dto;
                default:
                    return null;
            }
        }
        
        public static Range<decimal> AsNumericRange(object range)
        {
            if (range == null || !IsNumericRange(range))
                return null;
            
            dynamic r = range;
            
            // Using decimal for precision 
            var numeric = new Range<decimal>()
            {
                IsMaxInclusive = r.IsMaxInclusive,
                IsMinInclusive = r.IsMinInclusive,
                MinValue = r.MinValue == null
                    ? (decimal?) null 
                    : SmartConverter.Convert<decimal>(r.MinValue.ToString()),
                MaxValue = r.MaxValue == null
                    ? (decimal?) null 
                    : SmartConverter.Convert<decimal>(r.MaxValue.ToString()) 
            };

            return numeric;
        }
    }
}
