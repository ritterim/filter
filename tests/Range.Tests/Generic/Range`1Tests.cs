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
        public void Should_convert_object_that_is_date_range_to_datetimeoffset_range()
        {
            var dateTimeRange = new Range<DateTime>
            {
                MinValue = new DateTime(2000, 1, 1),
                MaxValue = new DateTime(2020, 1, 1)
            } as object;
            
            var result = Range.AsDateRange(dateTimeRange);
            
            Assert.Equal(2000, result.MinValue.Value.Year);
            Assert.Equal(2020, result.MaxValue.Value.Year);
        }
        
        [Fact]
        public void Should_convert_object_that_is_datetimeoffset_range_to_datetimeoffset_range()
        {
            var dateTimeRange = new Range<DateTimeOffset>
            {
                MinValue = new DateTime(2000, 1, 1),
                MaxValue = new DateTime(2020, 1, 1)
            } as object;
            
            var result = Range.AsDateRange(dateTimeRange);
            
            Assert.Equal(2000, result.MinValue.Value.Year);
            Assert.Equal(2020, result.MaxValue.Value.Year);
        }
        
        [Fact]
        public void Should_convert_object_that_is_int_range_to_decimal_range()
        {
            var dateTimeRange = new Range<int>
            {
                MinValue = 1,
                MaxValue = 10
            } as object;
            
            var result = Range.AsNumericRange(dateTimeRange);
            
            Assert.Equal(1, result.MinValue.Value);
            Assert.Equal(10, result.MaxValue.Value);
        }
        
        [Fact]
        public void Should_convert_null_to_null()
        {
            Assert.Null(Range.AsNumericRange(null));
            Assert.Null(Range.AsDateRange(null));
        }
        
        [Fact]
        public void Should_determine_object_as_numeric_range()
        {
            var numericRange = new Range<int>() as object;
            Assert.True(Range.IsNumericRange(numericRange));
        }
    }
}
