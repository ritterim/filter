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
        public static readonly IDictionary<Type, Func<object, string>> DefaultFilterValueFormatters =
            new Dictionary<Type, Func<object, string>>
            {
                { typeof(bool), (x) => x?.ToString().ToLowerInvariant() }
            };

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

            if (filterValueFormatters == null)
            {
                filterValueFormatters = DefaultFilterValueFormatters;
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
                        Func<object, string> formatter;
                        DefaultFilterValueFormatters.TryGetValue(filterProperty.PropertyType, out formatter);

                        if (formatter == null)
                        {
                            formatter = x => x.ToString();
                        }

                        mustQueries.Add(x =>
                            x.Match(y =>
                                y
                                .Field(validValuePropertyName)
                                .Query(formatter(filterPropertyValue))));
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
