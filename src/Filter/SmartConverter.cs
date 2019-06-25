using System;
using System.Collections.Generic;
using System.Linq;

namespace RimDev.Filter.Range
{
    public static class SmartConverter
    {
        private static List<Converter> globalConverters
            = new List<Converter>
            {
                new DateTimeMaxInclusiveConverter(),
                new DateTimeOffsetConverter(),
                new SystemConverter()
            };

        public static IReadOnlyList<Converter> Converters =>
            globalConverters.AsReadOnly();
        
        public static T Convert<T>(string str, ConvertingKind kind = ConvertingKind.Unspecified)
        {
            var ctx = new ConvertingContext(str, kind);

            foreach (var converter in globalConverters)
            {
                if (converter.TryConvert<T>(ctx, out var result))
                {
                    return result;
                }
            }

            return default(T);
        }

        public static void Add(params Converter[] converters)
        {
            if (converters == null) 
                throw new ArgumentNullException(nameof(converters));
            
            globalConverters.AddRange(converters.Where(x => x != null));
        }

        public static void Insert(int index, Converter converter)
        {
            if (converter == null) 
                throw new ArgumentNullException(nameof(converter));
            
            globalConverters.Insert(index, converter);
        }

        public static void Remove(int index)
        {
            globalConverters.RemoveAt(index);
        }
    }
}
