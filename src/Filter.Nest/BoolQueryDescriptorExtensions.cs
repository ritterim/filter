using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RimDev.Filter.Nest
{
    public static class BoolQueryDescriptorExtensions
    {
        public static BoolQueryDescriptor<T> Filter<T>(
            this BoolQueryDescriptor<T> value,
            object filter,
            IDictionary<Type, Func<object, string>> filterValueFormatters = null)
            where T : class
        {
            if (filter == null)
            {
                return value;
            }

            var mustQueries = FilterLogic.GenerateMustQueriesFromFilter<T>(filter, filterValueFormatters);

            if (mustQueries.Any())
            {
                return value.Filter(mustQueries);
            }

            return value;
        }
    }
}