using RimDev.Filter.Range.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RimDev.Filter.Range
{
    public static class Range
    {
        /// <summary>
        /// Try to parse an IRange<T> from a string. Throws FormatException if value is not valid.
        /// Note: Prefer using GetResultFromString<T>(string value) in performance critical scenarios
        /// to avoid most instances of control flow via exceptions.
        /// </summary>
        public static IRange<T> FromString<T>(string value)
            where T : struct
        {
            var rangeResult = Range.GetResultFromString<T>(value);

            if (rangeResult.Errors.Any())
            {
                throw new FormatException(string.Join(", ", rangeResult.Errors));
            }

            return rangeResult.Value;
        }

        /// <summary>
        /// Try to parse an IRange<T> from a string.
        /// Returns RangeResult<T> to avoid throwing an exception.
        public static RangeResult<T> GetResultFromString<T>(string value)
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
                return RangeResult<T>.Error("value does not match expected format.");
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
                    return RangeResult<T>.Error("value cannot be open-ended for both min and max-values.");
                }

                if (isMinInclusive && isMinInfinite)
                {
                    return RangeResult<T>.Error("value cannot have inclusive infinite lower-bound.");
                }

                if (isMaxInclusive && isMaxInfinite)
                {
                    return RangeResult<T>.Error("value cannot have inclusive infinite upper-bound.");
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
                        return RangeResult<T>.Error(
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
                        return RangeResult<T>.Error(
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

            if (range.MinValue.HasValue && range.MaxValue.HasValue)
            {
                var anyInclusiveRanges = range.IsMinInclusive || range.IsMaxInclusive;
                var compareResult = Comparer<T>.Default.Compare(range.MinValue.Value, range.MaxValue.Value);

                if (anyInclusiveRanges && compareResult > 0)
                    return RangeResult<T>.Error(
                        "Minimum value of range must be less than or equal to maximum value.");

                if (!anyInclusiveRanges && compareResult >= 0)
                    return RangeResult<T>.Error(
                        "Minimum value of range must be less than maximum value when range is non-inclusive.");
            }

            return RangeResult<T>.Success(range);
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
