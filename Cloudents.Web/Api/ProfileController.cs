using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        // GET
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            return
            Ok();
        }
    }
}