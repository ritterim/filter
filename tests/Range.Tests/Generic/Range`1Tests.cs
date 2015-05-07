using RimDev.Filter.Range.Generic;
using Xunit;

namespace RimDev.Filter.Range.Tests.Generic
{
    public class Range_1Tests
    {
        public class Constructor
        {
            [Fact]
            public void Should_hydrate_new_instance()
            {
                var range = new Range<int>()
                {
                    MinValue = 5,
                    MaxValue = 10,
                    IsMinInclusive = true,
                    IsMaxInclusive = true
                };

                var sut = new Range<int>(range);

                Assert.NotNull(sut);
                Assert.Equal(5, sut.MinValue);
                Assert.Equal(10, sut.MaxValue);
                Assert.Equal(true, sut.IsMinInclusive);
                Assert.Equal(true, sut.IsMaxInclusive);
            }
        }
    }
}
