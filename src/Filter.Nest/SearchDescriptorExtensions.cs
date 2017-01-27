using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RimDev.Filter.Nest
{
    public static class SearchDescriptorExtensions
    {
        public static SearchDescriptor<T> PostFilter<T>(
            this SearchDescriptor<T> value,
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
                value = value.PostFilter(x =>
                    x.Bool(y =>
                        y.Must(mustQueries)));
            }

            return value;
        }
    }
}