using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
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
        [ValidateModel, ValidateRecaptcha]
        public async Task<IActionResult> PostAsync(
            [FromBody] LoginRequest model,
            [FromServices] ISmsSender client,
            CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "email not found");
                return BadRequest(ModelState);
            }


            var result = await client.SendSmsAsync(user, token).ConfigureAwait(false);

            if (result)
            {
                return Ok();
            }
            ModelState.AddModelError(string.Empty, "email or password are invalid");
            return BadRequest(ModelState);
        }
    }
}