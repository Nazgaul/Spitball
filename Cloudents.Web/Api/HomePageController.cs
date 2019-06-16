using System.Reflection;
using Cloudents.Query;
using Microsoft.AspNetCore.Mvc;
using Cloudents.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<RegularUser> _userManager;

        public HomePageController(IQueryBus queryBus, UserManager<RegularUser> userManager)
        {
            _queryBus = queryBus;
            _userManager = userManager;
        }

        [HttpGet("version")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Version()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = await _userManager.GetUserAsync(User);
            //    if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            //    {
            //        return Unauthorized();
            //    }
            //}

            return Ok(new { version = Assembly.GetExecutingAssembly().GetName().Version.ToString(4) });
        }
    }
}
