using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public LoginController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        // GET

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post()
        {
            var user = await _userManager.FindByEmailAsync("ram@cloudents.com");
            await _signInManager.SignInAsync(user, false);

            return Ok();
        }
    }
}