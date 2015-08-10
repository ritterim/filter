using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RimDev.Filter.Range.Generic;

namespace RimDev.Filter.Generic
{
    public static class IQueryable_1Extensions
    {
        public static IQueryable<T> Filter<T>(
            this IQueryable<T> value,
            object filter)
        {
            if (filter == null)
            {
                return value;
            }

            var validValueProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead)
                .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

            var filterProperties = filter.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead);

            var queryableValue = value;
            foreach (var filterProperty in filterProperties)
            {
                PropertyInfo validValueProperty;
                validValueProperties.TryGetValue(filterProperty.Name, out validValueProperty);

                var filterPropertyValue = filterProperty.GetValue(filter, null);
                if (validValueProperty != null && filterPropertyValue != null)
                {
                    var validValuePropertyName = validValueProperty.Name;

                    if (typeof(IEnumerable).IsAssignableFrom(filterProperty.PropertyType) &&
                        filterProperty.PropertyType != typeof(string))
                    {
                        queryableValue = queryableValue.Contains(validValuePropertyName, (IEnumerable)filterPropertyValue);
                    }
                    else if (filterProperty.PropertyType.IsGenericType &&
                        typeof(IRange<>).IsAssignableFrom(filterProperty.PropertyType.GetGenericTypeDefinition()) ||
                        filterProperty.PropertyType.GetInterfaces()
                        .Where(x => x.IsGenericType)
                        .Any(x => x.GetGenericTypeDefinition() == typeof(IRange<>)))
                    {
                        var genericTypeArgument = filterPropertyValue.GetType().GenericTypeArguments.First();

                        if (genericTypeArgument == typeof(byte))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<byte>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(char))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<char>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(DateTime))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<DateTime>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(decimal))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<decimal>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(double))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<double>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(float))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<float>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(int))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<int>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(long))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<long>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(sbyte))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<sbyte>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(short))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<short>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(uint))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<uint>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(ulong))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<ulong>)filterPropertyValue);
                        }
                        else if (genericTypeArgument == typeof(ushort))
                        {
                            queryableValue = queryableValue.Range(validValuePropertyName, (IRange<ushort>)filterPropertyValue);
                        }
                    }
                    else
                    {
                        try
                        {
                            if (validValueProperty.PropertyType.IsGenericType &&
                                validValueProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                queryableValue = queryableValue.Where(validValuePropertyName, filterPropertyValue as Nullable);
                            }
                            else
                            {
                                queryableValue = queryableValue.Where(validValuePropertyName, filterPropertyValue);
                            }
                        }
                        catch (Exception) { }
                    }
                }
            }

            return queryableValue;
        }
        
        private static IQueryable<T> Contains<T>(
            this IQueryable<T> query,
            string property,
            IEnumerable values)
        {
            if (values == null || !values.Cast<object>().Any())
            {
                return query;
            }

            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var propertyExpression = (Expression)Expression.Property(parameterExpression, property);
            var constantExpression = Expression.Constant(values);
            var propertyExpressionIsNullable = propertyExpression.Type.IsGenericType
                                                && propertyExpression.Type.GetGenericTypeDefinition() == typeof(Nullable<>)
                                                && constantExpression.Type.GetElementType() != propertyExpression.Type;

            if (propertyExpressionIsNullable)
            {
                propertyExpression = Expression.Property(propertyExpression, "Value");
            }

            Expression callExpression = Expression.Call(
                typeof(Enumerable),
                "Contains",
                new[] { propertyExpression.Type },
                constantExpression,
                propertyExpression);

            if (propertyExpressionIsNullable)
            {
                var nullablePropertyExpression = Expression.Property(parameterExpression, property);
                var notEqual = Expression.NotEqual(nullablePropertyExpression, Expression.Constant(null, nullablePropertyExpression.Type));
                callExpression = Expression.AndAlso(notEqual, callExpression);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(callExpression, parameterExpression);

            return query.Where(lambda);
        }

        private static IQueryable<T> Where<T>(
            this IQueryable<T> query,
            string property,
            object value)
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var propertyExpression = Expression.Property(parameterExpression, property);
            var constantExpression = Expression.Constant(value);
            var equalExpression = Expression.Equal(propertyExpression, constantExpression);
            var callExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new[] { query.ElementType },
                query.Expression,
                Expression.Lambda<Func<T, bool>>(equalExpression, parameterExpression));

            return query.Provider.CreateQuery<T>(callExpression);
        }

        private static IQueryable<T> Range<T, TRange>(
            this IQueryable<T> query,
            string property,
            IRange<TRange> range)
            where TRange : struct
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var propertyExpression = Expression.Property(parameterExpression, property);

            Expression minConstantExpression = null;

            if (range.MinValue.HasValue)
            {
                var minValueExpression = Expression.Constant(range.MinValue);
                minConstantExpression = minValueExpression.Type != propertyExpression.Type
                    ? Expression.Convert(minValueExpression, propertyExpression.Type)
                    : (Expression)minValueExpression;
            }

            BinaryExpression minGreaterExpression = null;

            if (minConstantExpression != null)
            {
                minGreaterExpression = range.IsMinInclusive
                ? Expression.GreaterThanOrEqual(propertyExpression, minConstantExpression)
                : Expression.GreaterThan(propertyExpression, minConstantExpression);
            }

            Expression maxConstantExpression = null;

            if (range.MaxValue.HasValue)
            {
                var maxValueExpression = Expression.Constant(range.MaxValue);
                maxConstantExpression = maxValueExpression.Type != propertyExpression.Type
                    ? Expression.Convert(maxValueExpression, propertyExpression.Type)
                    : (Expression)maxValueExpression;
            }

            BinaryExpression maxLessExpression = null;

            if (maxConstantExpression != null)
            {
                maxLessExpression = range.IsMaxInclusive
                ? Expression.LessThanOrEqual(propertyExpression, maxConstantExpression)
                : Expression.LessThan(propertyExpression, maxConstantExpression);
            }

            Expression logicExpression;

            if (minGreaterExpression != null && maxLessExpression != null)
            {
                logicExpression = Expression.And(minGreaterExpression, maxLessExpression);
            }
            else if (minGreaterExpression != null)
            {
                logicExpression = minGreaterExpression;
            }
            else
            {
                logicExpression = maxLessExpression;
            }

            var lambdaExpression = Expression.Lambda<Func<T, bool>>(logicExpression, parameterExpression);
            var callExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new[] { query.ElementType },
                query.Expression,
                lambdaExpression);

            return query.Provider.CreateQuery<T>(callExpression);
        }
    }
}
