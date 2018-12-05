using Microsoft.AspNetCore.Mvc;
using RimDev.Filter.Range.Generic;

namespace Range.Web.Http.Sample.AspNetCore.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public ActionResult Index([FromQuery(Name = "value")] Range<int> value)
        {
            return Ok(value);
        }
    }
}
