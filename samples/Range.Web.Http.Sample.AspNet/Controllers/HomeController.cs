using RimDev.Filter.Range.Generic;
using System.Web.Http;

namespace Range.Web.Http.Sample.AspNet.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index([FromUri]Range<int> value)
        {
            return Json(value);
        }
    }
}
