namespace RimDev.Filter.Range
{
    public abstract class Converter
    {
        public abstract bool TryConvert<T>(ConvertingContext context, out T value);
    }
}