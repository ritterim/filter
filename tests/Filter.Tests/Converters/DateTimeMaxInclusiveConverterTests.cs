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

        [Theory]
        [InlineData("01/01/2019 1:00 AM", "2019-01-01T01:00:00.0000000")]
        [InlineData("01/01/2019", "2019-01-01T23:59:59.9990000")]
        [InlineData("01/01/2019 12:00 AM", "2019-01-01T00:00:00.0000000")]
        [InlineData("2019 12:00 AM", null)]
        [InlineData("2019 12:00 xxxxxx AM", null)]
        [InlineData("2019-01-01T11:59:00.0000000", "2019-01-01T11:59:00.0000000")]
        [InlineData("2019-01-01T", null)]
        [InlineData("", null)]
        [InlineData(null, null)]
        public void Can_convert_DateTime(
            string requestDate,
            string expectedDate
        )
        {
            var result = sut.TryConvert<DateTime?>(
                new ConvertingContext(
                    requestDate,
                    ConvertingKind.MaxInclusive
                ),
                out var value);

            DateTime? expectedDateTime = DateTime.TryParse(expectedDate, out _) ? 
                DateTime.Parse(expectedDate) : (DateTime?)null;

            Assert.Equal(expectedDateTime, value);

            output.WriteLine(value.ToString());
        }

        [Theory]
        [InlineData("08/12/1992 07.00.00 -05:00", null)]
        [InlineData("01/01/2019 1:00:00 AM +00:00", "2019-01-01T00:00:00.0000000")]
        [InlineData("5/1/2008 8:06:32 AM +01:00", "2008-05-01T08:06:32.0000000+01:00")]
        [InlineData("01/01/2019 1:00 AM", "2019-01-01T01:00:00.0000000-05:00")]
        [InlineData("01/01/2019", ":   2019-01-01T23:59:59.9990000-05:00")]
        [InlineData("01/01/2019 12:00 AM", "2019-01-01T00:00:00.0000000-05:00")]
        [InlineData("2019 12:00 AM", null)]
        [InlineData("2019 12:00 xxxxxx AM", null)]
        [InlineData("2019-01-01T11:59:00.0000000", "2019-01-01T11:59:00.0000000-05:00")]
        [InlineData("2019-01-01T", null)]
        [InlineData("", null)]
        [InlineData(null, null)]
        public void Can_convert_DateTimeOffSet(
            string requestDate,
            string expectedDate
        )
        {
            var result = sut.TryConvert<DateTimeOffset?>(
                new ConvertingContext(
                    requestDate,
                    ConvertingKind.MaxInclusive
                ),
                out var value);

            DateTimeOffset? expectedDateTime = DateTimeOffset.TryParse(expectedDate, out _) ? 
                DateTimeOffset.Parse(expectedDate) : (DateTimeOffset?)null;


            Assert.Equal(expectedDateTime, value);

            output.WriteLine(value.ToString());
        }
    }
}