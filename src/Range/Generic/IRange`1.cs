namespace RimDev.Filter.Range.Generic
{
    public interface IRange<out T>
    {
        T MinValue { get; }
        T MaxValue { get; }

        bool IsMinInclusive { get; }
        bool IsMaxInclusive { get; }
    }
}
