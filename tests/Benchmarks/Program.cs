using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using RimDev.Filter;
using RimDev.Filter.Range;
using RimDev.Filter.Range.Generic;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class RangeBenchmarks
    {
        [Benchmark]
        public IRange<int> FromStringT_Invalid()
        {
            // Unsure how this may impact the results
            try
            {
                return Range.FromString<int>("[1,abc]");
            }
            catch
            {
                return null;
            }
        }

        [Benchmark]
        public RangeResult<int> GetResultFromStringT_Invalid() => Range.GetResultFromString<int>("abc");

        [Benchmark]
        public IRange<int> FromStringT_Valid() => Range.FromString<int>("[1,2]");

        [Benchmark]
        public RangeResult<int> GetResultFromStringT_Valid() => Range.GetResultFromString<int>("[1,2]");
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<RangeBenchmarks>();
        }
    }
}
