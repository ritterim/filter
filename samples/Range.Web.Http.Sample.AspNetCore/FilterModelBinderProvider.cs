using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RimDev.Filter.Range.Generic;
using RimDev.Filter.Range.Web.Http.AspNetCore;

namespace Range.Web.Http.Sample.AspNetCore
{
    public class FilterModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            foreach (var type in RimDev.Filter.Filter.SupportedRangeTypes)
            {
                var rangeType = typeof(IRange<>).MakeGenericType(type);

                if (rangeType.IsAssignableFrom(context.Metadata.ModelType))
                {
                    var modelBinder = (IModelBinder)Activator.CreateInstance(typeof(RangeModelBinder<>).MakeGenericType(type));

                    return modelBinder;
                }
            }

            return null;
        }
    }
}
