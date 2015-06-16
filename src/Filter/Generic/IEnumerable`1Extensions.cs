using RimDev.Filter.Range.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RimDev.Filter.Generic
{
    public static class IEnumerable_1Extensions
    {
        public static IEnumerable<T> Filter<T>(
            this IEnumerable<T> value,
            object filter)
        {
            if (filter == null)
            {
                return value;
            }
            else
            {
                var validValueProperties = typeof(T).GetProperties(
                    BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanRead == true);

                var filterProperties = filter.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanRead == true);

                var queryableValue = value.AsQueryable();

                foreach (var filterProperty in filterProperties)
                {
                    var validValueProperty = validValueProperties
                        .Where(x => x.Name == filterProperty.Name)
                        .FirstOrDefault();
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
                                queryableValue = queryableValue.Where(validValuePropertyName, filterPropertyValue);
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }

                return queryableValue.AsEnumerable();
            }
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
            var propertyExpressionIsNullable =  propertyExpression.Type.IsGenericType 
                                                && propertyExpression.Type.GetGenericTypeDefinition() == typeof (Nullable<>)
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
                Expression.Lambda<Func<T, bool>>(equalExpression, new[] { parameterExpression }));

            return query.Provider.CreateQuery<T>(callExpression);
        }

        private static IQueryable<T> Range<T, TRange>(
            this IQueryable<T> query,
            string property,
            IRange<TRange> range)
            where TRange : struct
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var propertyExpression = Expression.Convert(
                Expression.Property(parameterExpression, property),
                typeof(TRange));

            ConstantExpression minConstantExpression = null;

            if (range.MinValue.HasValue)
            {
                minConstantExpression = Expression.Constant(range.MinValue);
            }

            BinaryExpression minGreaterExpression = null;

            if (minConstantExpression != null)
            {
                minGreaterExpression = range.IsMinInclusive
                ? Expression.GreaterThanOrEqual(propertyExpression, minConstantExpression)
                : Expression.GreaterThan(propertyExpression, minConstantExpression);
            }

            ConstantExpression maxConstantExpression = null;

            if (range.MaxValue.HasValue)
            {
                maxConstantExpression = Expression.Constant(range.MaxValue);
            }

            BinaryExpression maxLessExpression = null;

            if (maxConstantExpression != null)
            {
                maxLessExpression = range.IsMaxInclusive
                ? Expression.LessThanOrEqual(propertyExpression, maxConstantExpression)
                : Expression.LessThan(propertyExpression, maxConstantExpression);
            }

            Expression logicExpression = null;

            if (minGreaterExpression != null && maxLessExpression != null)
            {
                logicExpression = Expression.And(minGreaterExpression, maxLessExpression);
            }
            else if (minGreaterExpression != null && maxLessExpression == null)
            {
                logicExpression = minGreaterExpression;
            }
            else
            {
                logicExpression = maxLessExpression;
            }

            var callExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new[] { query.ElementType },
                query.Expression,
                Expression.Lambda<Func<T, bool>>(logicExpression, new[] { parameterExpression }));

            return query.Provider.CreateQuery<T>(callExpression);
        }
    }
}
