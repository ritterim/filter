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

                if (validValueProperty != null)
                {
                    var validValuePropertyName = validValueProperty.Name;

                    if (typeof(IEnumerable).IsAssignableFrom(filterProperty.PropertyType) &&
                        filterProperty.PropertyType != typeof(string))
                    {
                        queryableValue = queryableValue.Contains(validValuePropertyName, (IEnumerable)filterPropertyValue);
                    }
                    else if (typeof(Range<int>).IsAssignableFrom(filterProperty.PropertyType))
                    {
                        queryableValue = queryableValue.Range(validValuePropertyName, (Range<int>)filterPropertyValue);
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

        private static IQueryable<T> Contains<T>(
            this IQueryable<T> query,
            string property,
            IEnumerable values)
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var propertyExpression = Expression.Property(parameterExpression, property);
            var constantExpression = Expression.Constant(values);
            var callExpression = Expression.Call(
                typeof(Enumerable),
                "Contains",
                new[] { propertyExpression.Type },
                constantExpression,
                propertyExpression);
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
            Range<TRange> range)
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var propertyExpression = Expression.Property(parameterExpression, property);

            var minConstantExpression = Expression.Constant(range.MinValue);
            BinaryExpression minGreaterExpression = range.IsMinInclusive
                ? Expression.GreaterThanOrEqual(propertyExpression, minConstantExpression)
                : Expression.GreaterThan(propertyExpression, minConstantExpression);

            var maxConstantExpression = Expression.Constant(range.MaxValue);
            BinaryExpression maxLessExpression = range.IsMaxInclusive
                ? Expression.LessThanOrEqual(propertyExpression, maxConstantExpression)
                : Expression.LessThan(propertyExpression, maxConstantExpression);

            var logicExpression = Expression.And(minGreaterExpression, maxLessExpression);

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
