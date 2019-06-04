using System;
using System.Globalization;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class ForgotPasswordController : Controller
    {
        private readonly UserManager<RegularUser> _userManager;
        private readonly SignInManager<RegularUser> _signInManager;
        private readonly IQueueProvider _queueProvider;
        private const string EmailTempDictionaryKey = "EmailForgotPassword";
        private readonly IStringLocalizer<ForgotPasswordController> _localizer;

        public ForgotPasswordController(UserManager<RegularUser> userManager, SignInManager<RegularUser> signInManager,
            IQueueProvider queueProvider, IStringLocalizer<ForgotPasswordController> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _queueProvider = queueProvider;
            _localizer = localizer;
        }

        // GET
        [HttpPost]
        public async Task<IActionResult> Post(ForgotPasswordRequest model, [FromHeader] CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("ForgotPassword", _localizer["UserDoesntExists"]);
                return BadRequest(ModelState);
            }
            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            var phoneConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);
            var userLockedOut = await _userManager.GetLockoutEndDateAsync(user) ?? DateTimeOffset.MinValue;

            if (!emailConfirmed || !phoneConfirmed || userLockedOut == DateTimeOffset.MaxValue)
            {
                ModelState.AddModelError("ForgotPassword", _localizer["UserDoesntExists"]);
                return BadRequest(ModelState);
            }

            await GenerateEmailAsync(user, token);
            return Ok();
        }

        private async Task GenerateEmailAsync(RegularUser user, CancellationToken token)
        {
            TempData[EmailTempDictionaryKey] = user.Email;
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = UrlEncoder.Default.Encode(code);
            var link = Url.Link("ResetPassword", new { user.Id, code });
            var message = new ResetPasswordEmail(user.Email, link, CultureInfo.CurrentUICulture);
            await _queueProvider.InsertMessageAsync(message, token);
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendEmailAsync(
            CancellationToken token)
        {

            var email = TempData[EmailTempDictionaryKey];
            if (email == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(email.ToString());
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["UserDoesntExists"]);
                return BadRequest(ModelState);
            }

            await GenerateEmailAsync(user, token);
            return Ok();
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequest model, CancellationToken token)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest();
            }
            model.Code = System.Net.WebUtility.UrlDecode(model.Code);
            
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                if (user.EmailConfirmed && user.PhoneNumberConfirmed)
                {
                    await _signInManager.SignInAsync(user, false);
                    return Ok();
                }
                ModelState.AddModelError(string.Empty, _localizer["UserDoesntExists"]);
                return BadRequest(ModelState);

            }
            //TODO: Localize
            ModelState.AddIdentityModelError(result);
            return BadRequest(ModelState);
        }
    }
}