using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Storage;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
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
        private readonly SbSignInManager _signInManager;
        private readonly UserManager<User> _userManager;

        public LoginController(SbSignInManager signInManager, UserManager<User> userManager)
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

            var taskSignIn = _signInManager.SignInTwoFactorAsync(user, false);
            var taskSms = client.SendSmsAsync(user, token);
           // var taskSms = client.InsertMessageAsync(new SmsMessage(user.PhoneNumber, code), token);

            await Task.WhenAll(taskSms, taskSignIn).ConfigureAwait(false);
            if (taskSignIn.Result.RequiresTwoFactor)
            {
                return Ok();
            }
            
            ModelState.AddModelError(string.Empty,"Some error");
            return BadRequest(ModelState);
        }
    }
}