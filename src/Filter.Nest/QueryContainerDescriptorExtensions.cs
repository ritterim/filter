using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RimDev.Filter.Nest
{
    public static class QueryContainerDescriptorExtensions
    {
        public static QueryContainer Filter<T>(
            this QueryContainerDescriptor<T> value,
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
                return value.Bool(x =>
                    x.Must(mustQueries));
            }

            return value;
        }
    }
}