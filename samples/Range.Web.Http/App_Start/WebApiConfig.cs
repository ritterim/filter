using RimDev.Filter;
using RimDev.Filter.Range.Generic;
using RimDev.Filter.Range.Web.Http;
using System;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;

namespace Range.Web.Http
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            foreach (var type in Filter.SupportedRangeTypes)
            {
                var intRangeProvider = new SimpleModelBinderProvider(
                    typeof(Range<>).MakeGenericType(type),
                    (IModelBinder)Activator.CreateInstance(typeof(RangeModelBinder<>).MakeGenericType(type)));

                config.Services.Insert(typeof(ModelBinderProvider), 0, intRangeProvider);
            }
        }
    }
}
