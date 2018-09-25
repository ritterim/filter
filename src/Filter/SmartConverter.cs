using System;

namespace RimDev.Filter.Range
{
    public static class SmartConverter
    {
        public static T Convert<T>(string str)
        {
            dynamic dynamicValue = null;
            if (typeof(DateTimeOffset).IsAssignableFrom(typeof(T)))
            {
                dynamicValue = DateTimeOffset.Parse(str);
            }

            var result = dynamicValue ?? (T)System.Convert.ChangeType(str, typeof(T));

            return result;
        }
    }
}
