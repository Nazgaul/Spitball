using System;
using System.Reflection;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Query;
using Cloudents.Query.Query;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly UserManager<User> _userManager;

        public HomePageController(IQueryBus queryBus, UserManager<User> userManager)
        {
            _queryBus = queryBus;
            _userManager = userManager;
        }

        [HttpGet]
        [ResponseCache(Duration = TimeConst.Day, Location = ResponseCacheLocation.Any)]
        public async Task<ActionResult<StatsDto>> GetAsync(CancellationToken token)
        {
            var query = new HomePageQuery();
            return await _queryBus.QueryAsync(query, token);
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
