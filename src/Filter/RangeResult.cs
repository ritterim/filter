using System.Collections.Generic;
using System.Linq;
using RimDev.Filter.Range.Generic;

namespace RimDev.Filter
{
    public class RangeResult<T> where T : struct
    {
        public ICollection<string> Errors { get; set; } = new List<string>();

        public bool IsValid => !Errors.Any();

        public IRange<T> Value { get; set; }


        public static RangeResult<T> Success(IRange<T> value)
        {
            var result = new RangeResult<T>
            {
                Value = value
            };

            return result;
        }

        public static RangeResult<T> Error(string errorMessage)
        {
            var result = new RangeResult<T>();

            result.Errors.Add(errorMessage);

            return result;
        }
    }
}
