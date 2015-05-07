namespace RimDev.Filter.Range.Generic
{
    public class Range<T> : IRange<T>
    {
        public Range() { }

        public Range(IRange<T> value)
        {
            MinValue = value.MinValue;
            MaxValue = value.MaxValue;

            IsMinInclusive = value.IsMinInclusive;
            IsMaxInclusive = value.IsMaxInclusive;
        }

        public T MinValue { get; set; }
        public T MaxValue { get; set; }

        public bool IsMinInclusive { get; set; }
        public bool IsMaxInclusive { get; set; }
    }
}
