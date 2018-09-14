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
        public void Should_determine_object_as_date_range()
        {
            var dateTimeRange = new Range<DateTime>() as object;
            Assert.True(Range.IsDateRange(dateTimeRange));
        }
        
        [Fact]
        public void Should_determine_object_as_numeric_range()
        {
            var numericRange = new Range<int>() as object;
            Assert.True(Range.IsNumericRange(numericRange));
        }
    }
}
