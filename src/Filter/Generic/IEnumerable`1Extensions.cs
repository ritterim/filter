using System.Collections.Generic;
using System.Linq;

namespace RimDev.Filter.Generic
{
    public static class IEnumerable_1Extensions
    {
        public static IEnumerable<T> Filter<T>(
            this IEnumerable<T> value,
            object filter)
        {
            return value.AsQueryable().Filter(filter);
        }
    }
}
