using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nest;

namespace RimDev.Filter.Nest
{
    internal static class FilterLogic
    {
        internal static List<Func<QueryContainerDescriptor<T>, QueryContainer>> GenerateMustQueriesFromFilter<T>(object filter, IDictionary<Type, Func<object, string>> filterValueFormatters) where T : class
        {
            if (filterValueFormatters == null)
            {
                filterValueFormatters = NestFilterOptions.DefaultFilterValueFormatters;
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

                    if (typeof(Range.Generic.IRange<>).IsAssignableFrom(filterProperty.PropertyType))
                    {
                        
                        
                    }
                    else
                    if (typeof(IEnumerable).IsAssignableFrom(filterProperty.PropertyType)
                        && filterProperty.PropertyType != typeof(string))
                    {
                        foreach (var item in (IEnumerable)filterPropertyValue)
                        {
                            if (aliasAttribute != null)
                            {
                                queries.Add(x =>
                                    x.Match(y =>
                                        y
                                            .Field(aliasAttribute.Alias)
                                            .Query(item.ToString())));
                            }
                            else
                            {
                                queries.Add(x =>
                                    x.Match(y =>
                                        y
                                            .Field(validValueProperty)
                                            .Query(item.ToString())));
                            }
                        }
                    }
                    else
                    {
                        Func<object, string> formatter;
                        var type = Nullable.GetUnderlyingType(filterProperty.PropertyType) ?? filterProperty.PropertyType;
                        filterValueFormatters.TryGetValue(type, out formatter);

                        if (formatter == null)
                        {
                            formatter = x => x.ToString();
                        }

                        if (aliasAttribute != null)
                        {
                            mustQueries.Add(x =>
                                x.Match(y =>
                                    y
                                        .Field(aliasAttribute.Alias)
                                        .Query(formatter(filterPropertyValue))));
                        }
                        else
                        {
                            mustQueries.Add(x =>
                                x.Match(y =>
                                    y
                                        .Field(validValueProperty)
                                        .Query(formatter(filterPropertyValue))));
                        }
                    }

                    mustQueries.Add(x =>
                        x.Bool(y =>
                            y.Should(queries)));
                }
            }
            return mustQueries;
        }
    }
}
