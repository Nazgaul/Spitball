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

        private readonly ConfigurationService _versionService;

        public HomePageController(ConfigurationService versionService)
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
