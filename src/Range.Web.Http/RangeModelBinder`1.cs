using RimDev.Filter.Range.Generic;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace RimDev.Filter.Range.Web.Http
{
    public class RangeModelBinder<T> : IModelBinder
        where T : struct
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (!typeof(IRange<T>).IsAssignableFrom(bindingContext.ModelType))
            {
                return false;
            }

            var value = bindingContext.ValueProvider.GetValue(
                bindingContext.ModelName);

            if (value == null)
            {
                return true;
            }

            var rawValue = value.RawValue as string;

            if (rawValue == null)
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName,
                    "Invalid value-type.");

                return true;
            }

            try
            {
                bindingContext.Model = Range.FromString<T>(rawValue);

                return true;
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName,
                    ex.Message);

                return true;
            }
        }
    }
}
