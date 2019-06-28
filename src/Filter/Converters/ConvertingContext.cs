namespace RimDev.Filter.Range
{
    public class ConvertingContext
    {
        public ConvertingContext(string value, ConvertingKind kind = ConvertingKind.Unspecified)
        {
            Value = value;
            Kind = kind;
        }
        
        public ConvertingKind Kind { get; }
        public string Value { get; }
    }
}