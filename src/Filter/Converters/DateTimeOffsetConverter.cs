using System;

namespace RimDev.Filter.Range
{
    public class DateTimeOffsetConverter : Converter
    {
        public override bool TryConvert<T>(ConvertingContext context, out T value)
        {
            if (!typeof(DateTimeOffset).IsAssignableFrom(typeof(T)))
            {
                value = default(T);
                return false;
            }

            dynamic dynamicValue = DateTimeOffset.Parse(context.Value);
            // I'm cheating here, I know
            value = dynamicValue;

            return true;
        }
    }
}