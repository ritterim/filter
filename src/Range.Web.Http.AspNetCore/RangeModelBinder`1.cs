using Microsoft.AspNetCore.Mvc.ModelBinding;
using RimDev.Filter.Range.Generic;
using System.Threading.Tasks;

namespace RimDev.Filter.Range.Web.Http.AspNetCore
{
    public class RangeModelBinder<T> : IModelBinder
        where T : struct
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!typeof(IRange<T>).IsAssignableFrom(bindingContext.ModelType))
            {
                return Task.CompletedTask;
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(
                bindingContext.ModelName);

            if (valueProviderResult == null || valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var rawValue = valueProviderResult.FirstValue;

            if (rawValue == null)
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName,
                    "Invalid value-type.");

                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var rangeResult = Range.GetResultFromString<T>(rawValue);

            if (rangeResult.IsValid)
            {
                bindingContext.Result = ModelBindingResult.Success(rangeResult.Value);
            }
            else
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName,
                    string.Join(", ", rangeResult.Errors));

                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}
