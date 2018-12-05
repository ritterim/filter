using Owin;
using RimDev.Filter;
using RimDev.Filter.Range.Generic;
using RimDev.Filter.Range.Web.Http;
using System;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;

namespace Range.Web.Http.Sample.AspNet
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            foreach (var type in Filter.SupportedRangeTypes)
            {
                var modelBinderProvider = new SimpleModelBinderProvider(
                    typeof(Range<>).MakeGenericType(type),
                    (IModelBinder)Activator.CreateInstance(typeof(RangeModelBinder<>).MakeGenericType(type)));

                config.Services.Insert(typeof(ModelBinderProvider), 0, modelBinderProvider);
            }

            app.UseWebApi(config);
        }
    }
}