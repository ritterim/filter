using System;

namespace RimDev.Filter.Range.Generic
{
    public interface IRange<T>
        where T : struct
    {
        T? MinValue { get; }
        T? MaxValue { get; }

        bool IsMinInclusive { get; }
        bool IsMaxInclusive { get; }

        void Validate();
    }
}
