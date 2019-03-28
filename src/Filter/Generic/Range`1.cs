using System;
using System.Collections;
using System.Collections.Generic;

namespace RimDev.Filter.Range.Generic
{
    public class Range<T> : IRange<T>
        where T : struct
    {
        public Range() { }

        public Range(IRange<T> value)
        {
            MinValue = value.MinValue;
            MaxValue = value.MaxValue;

            IsMinInclusive = value.IsMinInclusive;
            IsMaxInclusive = value.IsMaxInclusive;
        }

        public T? MinValue { get; set; }
        public T? MaxValue { get; set; }

        public bool IsMinInclusive { get; set; }
        public bool IsMaxInclusive { get; set; }

        public static implicit operator string(Range<T> range)
        {
            return range.ToString();
        }

        public static implicit operator Range<T>(string range)
        {
            return Range.FromString<T>(range) as Range<T>;
        }

        public override string ToString()
        {
            return string.Format("{0}{1},{2}{3}",
                    IsMinInclusive ? "[" : "(",
                    MinValue,
                    MaxValue,
                    IsMaxInclusive ? "]" : ")"
                );
        }
    }
}
