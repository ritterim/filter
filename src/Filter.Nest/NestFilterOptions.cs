using System;
using System.Collections.Generic;

namespace RimDev.Filter.Nest
{
    public static class NestFilterOptions
    {
        public static readonly IDictionary<Type, Func<object, string>> DefaultFilterValueFormatters =
            new Dictionary<Type, Func<object, string>>
            {
                { typeof(bool), (x) => x?.ToString().ToLowerInvariant() }
            };
    }
}