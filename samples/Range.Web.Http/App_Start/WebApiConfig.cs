using RimDev.Filter.Range.Generic;
using RimDev.Filter.Range.Web.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;

namespace Range.Web.Http
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var intRangeProvider = new SimpleModelBinderProvider(
                typeof(Range<int>),
                new RangeModelBinder<int>());

            config.Services.Insert(typeof(ModelBinderProvider), 0, intRangeProvider);
        }
    }
}
