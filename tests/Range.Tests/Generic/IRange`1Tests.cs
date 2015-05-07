using RimDev.Filter.Range.Generic;
using Xunit;

namespace RimDev.Filter.Range.Tests.Generic
{
    public class IRange_1Tests
    {
        /// <summary>
        /// This test is a compile-time check to confirm
        /// IRange<T> is still covariant.
        /// </summary>
        [Fact]
        public void Should_allow_covariance()
        {
            IRange<string> intRange = new Range<string>();

            IRange<object> objectRange = new Range<object>();

            objectRange = intRange;

            // Not really necessary.
            Assert.NotNull(objectRange);
        }
    }
}
