using System;

namespace RimDev.Filter.Range
{
    public class SystemConverter : Converter
    {
        public override bool TryConvert<T>(ConvertingContext context, out T value)
        {
            // this will let exceptions bubble up
            value = (T) Convert.ChangeType(context.Value, typeof(T));
            return true;
        }
    }
}