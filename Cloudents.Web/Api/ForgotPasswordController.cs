using Cloudents.Core.Entities;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class ForgotPasswordController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IQueueProvider _queueProvider;
        private const string EmailTempDictionaryKey = "EmailForgotPassword";
        private readonly IStringLocalizer<ForgotPasswordController> _localizer;

        public ForgotPasswordController(UserManager<User> userManager, SignInManager<User> signInManager,
            IQueueProvider queueProvider, IStringLocalizer<ForgotPasswordController> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _queueProvider = queueProvider;
            _localizer = localizer;
        }

        // GET
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostAsync(ForgotPasswordRequest model, [FromHeader] CancellationToken token)
        {
            if (User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("ForgotPassword", "User is signed in");
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("ForgotPassword", _localizer["UserDoesntExists"]);
                return BadRequest(ModelState);
            }
            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            var phoneConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);
            var userLockedOut = await _userManager.GetLockoutEndDateAsync(user) ?? DateTimeOffset.MinValue;

            if (phoneConfirmed && !emailConfirmed)
            {
                await GenerateEmailAsync(user, token);
                return Ok();
            }

            if (!emailConfirmed || !phoneConfirmed || userLockedOut == DateTimeOffset.MaxValue)
            {
                ModelState.AddModelError("ForgotPassword", _localizer["UserDoesntExists"]);
                return BadRequest(ModelState);
            }

            await GenerateEmailAsync(user, token);
            return Ok();
        }

        private async Task GenerateEmailAsync(User user, CancellationToken token)
        {
            TempData[EmailTempDictionaryKey] = user.Email;
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = UrlEncoder.Default.Encode(code);
            var link = Url.Link(Controllers.ForgotPasswordController.ResetPasswordRouteName, new { user.Id, code });
            var message = new ResetPasswordEmail(user.Email, link, CultureInfo.CurrentUICulture);
            await _queueProvider.InsertMessageAsync(message, token);
        }

        [HttpPost("resend")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ResetPasswordAsync([FromBody]ResetPasswordRequest model, [FromHeader(Name = "referer")] Uri referer, CancellationToken token)
        {
            var queryString = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(referer.Query);
            if (queryString.TryGetValue("id", out var id) &&
                queryString.TryGetValue("code", out var code)
            )
            {
                //  var id = queryString["id"];
                //var code = queryString["code"];
                //var from = queryString["byPass"];
                if (id == StringValues.Empty || code == StringValues.Empty)
                {
                    return BadRequest();
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return BadRequest();
                }

                user.EmailConfirmed = true;

                code = System.Net.WebUtility.UrlDecode(code);

                var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
                if (result.Succeeded)
                {
                    //if (from != null)
                    //{
                    //    await _signInManager.SignInAsync(user, false);
                    //    return Ok();
                    //}
                    if (user.PhoneNumberConfirmed)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return Ok();
                    }

                    return Ok();
                    //ModelState.AddModelError(nameof(model.Password), _localizer["UserDoesntExists"]);
                    //return BadRequest(ModelState);

                }
                //TODO: Localize
                ModelState.AddIdentityModelError(nameof(model.Password), result);
                return BadRequest(ModelState);
            }
            return BadRequest();
        }
    }
}