using System;
using System.Globalization;
using System.Linq;

namespace RimDev.Filter.Range
{
    /// <summary>
    /// Handles Inclusive Date Ranges ([2019-01-01,2019-
    /// </summary>
    public class DateTimeMaxInclusiveConverter : Converter
    {        
        public override bool TryConvert<T>(ConvertingContext context, out T value)
        {
            if (context.Kind == ConvertingKind.MaxInclusive)
            {
                dynamic dynamicValue;
                if (typeof(DateTimeOffset).IsAssignableFrom(typeof(T)))
                {
                    if (DateTimeOffset.TryParse(context.Value, out var parsed))
                    {
                        // has no time
                        if (!HasExplicitTime(parsed.Date, context.Value))
                        {
                            // set the time to the latest moment
                            parsed = new DateTimeOffset(
                                    parsed.Year,
                                    parsed.Month,
                                    parsed.Day,
                                    23,
                                    59,
                                    59,
                                    parsed.Offset)
                                .AddMilliseconds(999);
                        }

                        // I'm cheating here, I know
                        dynamicValue = parsed;
                        value = dynamicValue;
                        return true;
                    }
                }
                else if (typeof(DateTime).IsAssignableFrom(typeof(T)))
                {
                    if (DateTime.TryParse(context.Value, out var parsed))
                    {
                        // has no time
                        if (!HasExplicitTime(parsed, context.Value))
                        {
                            // set the time to the latest moment
                            parsed = new DateTime(
                                parsed.Year,
                                parsed.Month,
                                parsed.Day,
                                23,
                                59,
                                59,
                                999
                            );
                        }

                        // I'm cheating here, I know
                        dynamicValue = parsed;
                        value = dynamicValue;
                        return true;
                    }
                }
            }

            value = default(T);
            return false;
        }
        
        /// <summary>
        /// https://stackoverflow.com/a/28742687
        /// </summary>
        /// <param name="parsedTimestamp"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private bool HasExplicitTime(DateTime parsedTimestamp, string timestamp)
        {
            string[] dateTimeSeparators = { "T", " ", "@" };
            string[] timeSeparators = {
                CultureInfo.CurrentUICulture.DateTimeFormat.TimeSeparator,
                CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator,
                ":"};

            if (parsedTimestamp.TimeOfDay.TotalSeconds != 0)
                return true;

            string[] dateOrTimeParts = timestamp.Split(
                dateTimeSeparators,
                StringSplitOptions.RemoveEmptyEntries);
            bool hasTimePart = dateOrTimeParts.Any(part =>
                part.Split(
                    timeSeparators,
                    StringSplitOptions.RemoveEmptyEntries).Length > 1);
            return hasTimePart;
        }

    }
}