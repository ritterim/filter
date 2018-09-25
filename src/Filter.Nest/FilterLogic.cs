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

                    if (filterPropertyValue.GetType().GetInterfaces().Any(
                        x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(Range.Generic.IRange<>)))
                    {
                        Range.Generic.Range<DateTimeOffset> dateRange;
                        Range.Generic.Range<decimal> numericRange;

                        if ((dateRange = Range.Range.AsDateRange(filterPropertyValue)) != null)
                        {
                            queries.Add(x =>
                                x.DateRange(y =>
                                {
                                    if (dateRange.MinValue.HasValue)
                                    {
                                        var minValue = dateRange.MinValue.Value.Date;

                                        if (aliasAttribute != null)
                                        {
                                            y = dateRange.IsMinInclusive
                                                ? y.Field(aliasAttribute.Alias).GreaterThanOrEquals(minValue)
                                                : y.Field(aliasAttribute.Alias).GreaterThan(minValue);
                                        }
                                        else
                                        {
                                            y = dateRange.IsMinInclusive
                                                ? y.Field(validValueProperty).GreaterThanOrEquals(minValue)
                                                : y.Field(validValueProperty).GreaterThan(minValue);
                                        }
                                    }

                                    if (dateRange.MaxValue.HasValue)
                                    {
                                        var maxValue = dateRange.MaxValue.Value.Date;

                                        if (aliasAttribute != null)
                                        {
                                            y = dateRange.IsMaxInclusive
                                                ? y.Field(aliasAttribute.Alias).LessThanOrEquals(maxValue)
                                                : y.Field(aliasAttribute.Alias).LessThan(maxValue);
                                        }
                                        else
                                        {
                                            y = dateRange.IsMaxInclusive
                                                ? y.Field(validValueProperty).LessThanOrEquals(maxValue)
                                                : y.Field(validValueProperty).LessThan(maxValue);
                                        }
                                    }

                                    return y;
                                }));
                        }
                        else if ((numericRange = Range.Range.AsNumericRange(filterPropertyValue)) != null)
                        {
                            queries.Add(x =>
                                x.Range(y =>
                                {
                                    if (numericRange.MinValue.HasValue)
                                    {
                                        var minValue = Convert.ToDouble(numericRange.MinValue.Value);

                                        if (aliasAttribute != null)
                                        {
                                            y = numericRange.IsMinInclusive
                                                ? y.Field(aliasAttribute.Alias).GreaterThanOrEquals(minValue)
                                                : y.Field(aliasAttribute.Alias).GreaterThan(minValue);
                                        }
                                        else
                                        {
                                            y = numericRange.IsMinInclusive
                                                ? y.Field(validValueProperty).GreaterThanOrEquals(minValue)
                                                : y.Field(validValueProperty).GreaterThan(minValue);
                                        }
                                    }

                                    if (numericRange.MaxValue.HasValue)
                                    {
                                        var maxValue = Convert.ToDouble(numericRange.MaxValue.Value);

                                        if (aliasAttribute != null)
                                        {
                                            y = numericRange.IsMaxInclusive
                                                ? y.Field(aliasAttribute.Alias).LessThanOrEquals(maxValue)
                                                : y.Field(aliasAttribute.Alias).LessThan(maxValue);
                                        }
                                        else
                                        {
                                            y = numericRange.IsMaxInclusive
                                                ? y.Field(validValueProperty).LessThanOrEquals(maxValue)
                                                : y.Field(validValueProperty).LessThan(maxValue);
                                        }
                                    }

                                    return y;
                                }));
                        }
                        else
                        {
                            throw new InvalidOperationException($"NEST filtering does not work on `Range<T>` where `T` is `{filterProperty.PropertyType.Name}`.");
                        }
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(filterProperty.PropertyType)
                        && filterProperty.PropertyType != typeof(string))
                    {
                        foreach (var item in (IEnumerable)filterPropertyValue)
                        {
                            if (item == null)
                                continue;

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
