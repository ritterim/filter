using Microsoft.AspNetCore.Mvc.ModelBinding;
using RimDev.Filter.Range.Generic;
using System;
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

            try
            {
                var rangeResult = RimDev.Filter.Range.Range.FromString<T>(rawValue);
                bindingContext.Result = ModelBindingResult.Success(rangeResult);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName,
                    ex.Message);

                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }
        }
    }
}
