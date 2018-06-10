using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class SmsController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public SmsController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost, ValidateModel]
        public async Task<IActionResult> SmsUserAsync(
            [FromBody]PhoneNumberRequest model,
            [FromServices] ISmsSender client,
            CancellationToken token)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (!user.EmailConfirmed)
            {
                return Unauthorized();
            }

            if (user.PhoneNumberConfirmed)
            {
                return Unauthorized();
            }
            var retVal = await _userManager.SetPhoneNumberAsync(user, model.Number).ConfigureAwait(false);
            if (retVal.Succeeded)
            {
                var result = await client.SendSmsAsync(user, token).ConfigureAwait(false);

                if (result)
                {
                    return Ok();
                }

                ModelState.AddModelError(string.Empty, "Invalid phone number");
                return BadRequest(ModelState);
            }

            if (retVal.Errors.Any(a => a.Code == "Duplicate"))
            {
                ModelState.AddModelError(string.Empty, "Phone number is already in use");
            }
            else
            {
                ModelState.AddIdentityModelError(retVal);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("verify"), ValidateModel]
        public async Task<IActionResult> VerifySmsAsync([FromBody]CodeRequest model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            var v = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, model.Number).ConfigureAwait(false);
            if (v.Succeeded)
            {
                await _signInManager.SignInAsync(user, false).ConfigureAwait(false);
                return Ok();
            }
            ModelState.AddIdentityModelError(v);
            return BadRequest(ModelState);
        }
    }
}