namespace RimDev.Filter.Range.Generic
{
    public class Range<T>
    {
        public T MinValue { get; set; }
        public T MaxValue { get; set; }

        public bool IsMinInclusive { get; set; }
        public bool IsMaxInclusive { get; set; }
    }
}
