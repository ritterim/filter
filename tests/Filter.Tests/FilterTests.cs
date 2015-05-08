using System;
using System.Collections.Generic;
using Xunit;

namespace RimDev.Filter.Tests
{
    public class FilterTests
    {
        public class SupportedRangeTypes
        {
            [Fact]
            public void Should_contain_only_the_following_types()
            {
                var supportedTypes = new List<Type>()
                {
                    typeof(byte),
                    typeof(char),
                    typeof(decimal),
                    typeof(double),
                    typeof(float),
                    typeof(int),
                    typeof(long),
                    typeof(sbyte),
                    typeof(short),
                    typeof(uint),
                    typeof(ulong),
                    typeof(ushort),
                };

                Assert.Equal(supportedTypes, Filter.SupportedRangeTypes);
            }
        }
    }
}
