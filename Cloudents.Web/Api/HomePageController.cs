using System.Reflection;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    {

        private readonly VersionService _versionService;

        public HomePageController(VersionService versionService)
        {
            _versionService = versionService;
        }

        [HttpGet("version")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Version()
        {
            return Ok(new { version = _versionService.GetVersion() });
        }
    }
}
