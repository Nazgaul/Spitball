using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public RegisterController(
            SignInManager<User> signInManager, 
            UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateUserAsync(RegisterEmailRequest model)
        {
            var user = new User
            {
                Email = model.Email,
                Name = model.Email
            };

            var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
            if (!p.Succeeded)
            {
                return BadRequest(p.Errors);
            }

            await _signInManager.SignInAsync(user, true).ConfigureAwait(false);
            return Ok();
        }
    }
}