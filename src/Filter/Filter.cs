using System;
using System.Collections.Generic;

namespace RimDev.Filter
{
    public static class Filter
    {
        public static IEnumerable<Type> SupportedRangeTypes = new List<Type>()
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
    }
}
