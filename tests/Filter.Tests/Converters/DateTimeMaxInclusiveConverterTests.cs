using System;
using RimDev.Filter.Range;
using Xunit;
using Xunit.Abstractions;

namespace Filter.Tests.Converters
{
    public class DateTimeMaxInclusiveConverterTests
    {
        private readonly ITestOutputHelper output;
        private DateTimeMaxInclusiveConverter sut = new DateTimeMaxInclusiveConverter();

        public DateTimeMaxInclusiveConverterTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Fact]
        public void Can_convert_datetime_to_maximum_moment()
        {
            var result = sut.TryConvert<DateTime>(
                new ConvertingContext(
                    "01/01/2019",
                    ConvertingKind.MaxInclusive
                ), 
                out var value);
            
            Assert.True(result);
            Assert.Equal(23, value.Hour);
            Assert.Equal(59, value.Minute);
            Assert.Equal(59, value.Second);
            Assert.Equal(999, value.Millisecond);
            
            output.WriteLine(value.ToString());
        }
        
        [Fact]
        public void Can_convert_datetime_without_affecting_time()
        {
            var result = sut.TryConvert<DateTime>(
                new ConvertingContext(
                    "01/01/2019 12:00 AM",
                    ConvertingKind.MaxInclusive
                ), 
                out var value);
            
            Assert.True(result);
            Assert.Equal(0, value.Hour);
            Assert.Equal(0, value.Minute);
            Assert.Equal(0, value.Second);
            Assert.Equal(0, value.Millisecond);
            
            output.WriteLine(value.ToString());
        }
        
        [Fact]
        public void Can_convert_datetimeoffset_to_maximum_moment()
        {
            var result = sut.TryConvert<DateTimeOffset>(
                new ConvertingContext(
                    "01/01/2019",
                    ConvertingKind.MaxInclusive
                ), 
                out var value);
            
            Assert.True(result);
            Assert.Equal(23, value.Hour);
            Assert.Equal(59, value.Minute);
            Assert.Equal(59, value.Second);
            Assert.Equal(999, value.Millisecond);
            
            output.WriteLine(value.ToString());
        }
        
        [Fact]
        public void Can_convert_datetimeoffset_without_affecting_time()
        {
            var result = sut.TryConvert<DateTimeOffset>(
                new ConvertingContext(
                    "01/01/2019 12:00 AM",
                    ConvertingKind.MaxInclusive
                ), 
                out var value);
            
            Assert.True(result);
            Assert.Equal(0, value.Hour);
            Assert.Equal(0, value.Minute);
            Assert.Equal(0, value.Second);
            Assert.Equal(0, value.Millisecond);
            
            output.WriteLine(value.ToString());
        }
        
        [Fact]
        public void Can_convert_datetimeoffset_without_affecting_time_one_am()
        {
            var result = sut.TryConvert<DateTimeOffset>(
                new ConvertingContext(
                    "01/01/2019 1:00 AM",
                    ConvertingKind.MaxInclusive
                ), 
                out var value);
            
            Assert.True(result);
            Assert.Equal(1, value.Hour);
            Assert.Equal(0, value.Minute);
            Assert.Equal(0, value.Second);
            Assert.Equal(0, value.Millisecond);
            
            output.WriteLine(value.ToString());
        }
    }
}