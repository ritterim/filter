using System;
using RimDev.Filter.Range.Generic;
using Xunit;

namespace RimDev.Filter.Range.Tests.Generic
{
    public class Range_1Tests
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
            Assert.True(sut.IsMinInclusive);
            Assert.True(sut.IsMaxInclusive);
        }
    
        [Fact]
        public void Should_determine_range_as_date_range()
        {
            var dateTimeRange = new Range<DateTime>();
            var dateTimeOffsetRange = new Range<DateTimeOffset>();
            var intRange = new Range<int>();

            Assert.True(dateTimeRange.IsDateRange());
            Assert.True(dateTimeOffsetRange.IsDateRange());
            Assert.False(intRange.IsDateRange());
        }
        
        [Fact]
        public void Should_determine_range_as_numeric_range()
        {
            var floatRange = new Range<float>();
            var intRange = new Range<int>();
            var dateRange = new Range<DateTime>();

            Assert.True(floatRange.IsNumericRange());
            Assert.True(intRange.IsNumericRange());
            Assert.False(dateRange.IsNumericRange());
        }
    }
}
