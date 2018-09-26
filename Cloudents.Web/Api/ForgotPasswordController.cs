using Cloudents.Core.Entities.Db;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class ForgotPasswordController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IServiceBusProvider _queueProvider;
        private const string EmailTempDictionaryKey = "EmailForgotPassword";
        private readonly IStringLocalizer<ForgotPasswordController> _localizer;

        public ForgotPasswordController(UserManager<User> userManager, SignInManager<User> signInManager,
            IServiceBusProvider queueProvider, IStringLocalizer<ForgotPasswordController> localizer)
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
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
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
            var link = Url.Link("ResetPassword", new { user.Id, code });
            var message = new ResetPasswordEmail(user.Email, link);
            await _queueProvider.InsertMessageAsync(message, token).ConfigureAwait(false);
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
            var user = await _userManager.FindByEmailAsync(email.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["UserDoesntExists"]);
                return BadRequest(ModelState);
            }

            await GenerateEmailAsync(user, token).ConfigureAwait(false);
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
                await _signInManager.SignInAsync(user, false);

                return Ok();
            }
            //TODO: Localize
            ModelState.AddIdentityModelError(result);
            return BadRequest(ModelState);
        }
    }
}