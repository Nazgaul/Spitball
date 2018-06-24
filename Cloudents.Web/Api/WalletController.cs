using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : Controller
    {
        // GET
        [HttpGet]
        public IActionResult Get()
        {

            return
            Ok();
        }
    }
}