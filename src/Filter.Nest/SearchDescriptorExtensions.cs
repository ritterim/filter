using Nest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RimDev.Filter.Nest
{
    public static class SearchDescriptorExtensions
    {
        public static SearchDescriptor<T> PostFilter<T>(
            this SearchDescriptor<T> value,
            object filter)
            where T : class
        {
            if (filter == null)
            {
                return value;
            }
            var validValueProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead)
                .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

            var filterProperties = filter
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead);

            var mustQueries = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();

            foreach (var filterProperty in filterProperties)
            {
                PropertyInfo validValueProperty;
                validValueProperties.TryGetValue(filterProperty.Name, out validValueProperty);

                var filterPropertyValue = filterProperty.GetValue(filter, null);

                if (validValueProperty != null && filterPropertyValue != null)
                {
                    var queries = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
                    var aliasAttribute = validValueProperty.GetCustomAttribute<MappingAliasAttribute>();
                    var validValuePropertyName = aliasAttribute != null
                        ? aliasAttribute.Alias
                        : filterProperty.Name;

                    if (typeof(IEnumerable).IsAssignableFrom(filterProperty.PropertyType)
                        && filterProperty.PropertyType != typeof(string))
                    {
                        foreach (var item in (IEnumerable)filterPropertyValue)
                        {
                            queries.Add(x =>
                                x.Match(y =>
                                    y
                                    .Field(validValuePropertyName)
                                    .Query(item.ToString())));
                        }
                    }
                    else
                    {
                        mustQueries.Add(x =>
                            x.Match(y =>
                                y
                                .Field(validValuePropertyName)
                                .Query(filterPropertyValue.ToString())));
                    }

                    mustQueries.Add(x =>
                        x.Bool(y =>
                            y.Should(queries)));
                }
            }

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
