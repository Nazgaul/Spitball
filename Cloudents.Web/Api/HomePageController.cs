using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    { 
      

        [HttpGet("version")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Version()
        {
            return Ok(new { version = Assembly.GetExecutingAssembly().GetName().Version.ToString(4) });
        }
    }
}
