using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace RimDev.Filter.NPoco
{
    /**
     * Source: http://graemehill.ca/entity-framework-dynamic-queries-and-parameterization/
     */
    internal static class ExpressionHelper
    {
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "This method is invoked dynamically.")]
        public static MemberExpression WrappedConstant<TValue>(TValue value)
        {
            var wrapper = new WrappedObj<TValue>(value);
            return Expression.Field(
                Expression.Constant(wrapper),
                typeof(WrappedObj<TValue>).GetField("Value"));
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        private struct WrappedObj<TValue>
        {
            [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "This member is dynamically accessed.")]
            public readonly TValue Value;

            public WrappedObj(TValue value)
            {
                Value = value;
            }
        }
    }
}
