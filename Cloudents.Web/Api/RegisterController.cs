using Cloudents.Web.Controllers;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class RegisterController : Controller
    {
        private readonly UserManager<RegularUser> _userManager;
        private readonly SbSignInManager _signInManager;

        private readonly IQueueProvider _queueProvider;
        private readonly ISmsSender _client;
        private readonly IStringLocalizer<RegisterController> _localizer;
        private readonly IStringLocalizer<LogInController> _loginLocalizer;
        private readonly ILogger _logger;

        internal const string Email = "email2";
        private const string EmailTime = "EmailTime";

        public RegisterController(UserManager<RegularUser> userManager, SbSignInManager signInManager,
             IQueueProvider queueProvider, ISmsSender client, IStringLocalizer<RegisterController> localizer, IStringLocalizer<LogInController> loginLocalizer, ILogger logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _queueProvider = queueProvider;
            _client = client;
            _localizer = localizer;
            _loginLocalizer = loginLocalizer;
            _logger = logger;
        }

        [HttpPost, ValidateRecaptcha, ValidateEmail]
        public async Task<ActionResult<ReturnSignUserResponse>> Post(
            [FromBody]RegisterRequest model,
            [CanBeNull] ReturnUrlRequest returnUrl,
            CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                try
                {
                    return await MakeDecision(user, false, returnUrl, token);
                }
                catch (ArgumentException)
                {
                }
               
                ModelState.AddModelError(nameof(model.Email), _localizer["UserExists"]);
                return BadRequest(ModelState);
            }
            user = CreateUser(model.Email, null);
            var p = await _userManager.CreateAsync(user, model.Password);
            if (p.Succeeded)
            {
                await GenerateEmailAsync(user, returnUrl, token);
                return new ReturnSignUserResponse(NextStep.EmailConfirmed, true);
            }
            ModelState.AddIdentityModelError(p);
            return BadRequest(ModelState);
        }


        private async Task<ReturnSignUserResponse> MakeDecision(RegularUser user,
            bool isExternal,
            [CanBeNull] ReturnUrlRequest returnUrl,
            CancellationToken token)
        {
            if (user.PhoneNumberConfirmed)
            {
                if (isExternal)
                {
                    await _signInManager.SignInAsync(user, false);
                    return new ReturnSignUserResponse(false);
                }

                throw new ArgumentException();
            }

            if (user.PhoneNumber != null)
            {
                var t1 = _signInManager.TempSignIn(user);
                var t2 = _client.SendSmsAsync(user, token);

                await Task.WhenAll(t1, t2);
                return new ReturnSignUserResponse(NextStep.VerifyPhone, true);
            }

            if (user.EmailConfirmed)
            {
                await _signInManager.TempSignIn(user);
                return new ReturnSignUserResponse(NextStep.EnterPhone, true);
            }
           
            await GenerateEmailAsync(user, returnUrl, token);
            return new ReturnSignUserResponse(NextStep.EmailConfirmed, true);
        }


        [HttpPost("google")]
        public async Task<ActionResult<ReturnSignUserResponse>> GoogleSignInAsync([FromBody] GoogleTokenRequest model,
            [FromServices] IGoogleAuth service,
            CancellationToken cancellationToken)
        {
            var result = await service.LogInAsync(model.Token, cancellationToken);
            _logger.Info($"received google user {result}");
            if (result == null)
            {
                ModelState.AddModelError("Google", _localizer["GoogleNoResponse"]);
                return BadRequest(ModelState);
            }

            var result2 = await _signInManager.ExternalLoginSignInAsync("Google", result.Id, false, true);
            if (result2.Succeeded)
            {
                return new ReturnSignUserResponse(false);
            }
            if (result2.IsLockedOut)
            {
                ModelState.AddModelError("Google", _loginLocalizer["LockOut"]);
                return BadRequest(ModelState);

            }

            var user = await _userManager.FindByEmailAsync(result.Email);

            if (user == null)
            {
                user = CreateUser(result.Email, result.Name);
                user.EmailConfirmed = true;
                user.ChangeLanguage(result.Language);

                var result3 = await _userManager.CreateAsync(user);
                if (result3.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", result.Id, result.Name));
                    return await MakeDecision(user, true, null, cancellationToken);
                }
                ModelState.AddModelError("Google", _localizer["GoogleUserRegisteredWithEmail"]);
                return BadRequest(ModelState);
            }
            if (!user.EmailConfirmed)
            {

                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
            await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", result.Id, result.Name));
            return await MakeDecision(user, true, null, cancellationToken);
        }


        private RegularUser CreateUser(string email, string name)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(name))
            {
                name = email.Split(new[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
           
            return new RegularUser(email, $"{name}.{GenerateRandomNumber()}",  CultureInfo.CurrentCulture);
        }

        private static int GenerateRandomNumber()
        {
            var rdm = new Random();
            return rdm.Next(1000, 9999);
        }

        private async Task GenerateEmailAsync(RegularUser user, [CanBeNull] ReturnUrlRequest returnUrl, CancellationToken token)
        {
            //if (user.OldUser.GetValueOrDefault() && user.SecurityStamp == null)
            //{
            //    await _userManager.UpdateSecurityStampAsync(user);
            //}
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = UrlEncoder.Default.Encode(code);
            var url = returnUrl?.Url;
            if (!Url.IsLocalUrl(url))
            {
                url = null;
            }
            TempData[EmailTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            var link = Url.Link("ConfirmEmail", new { user.Id, code, returnUrl = url, referral = TempData[HomeController.Referral] });
            TempData[Email] = user.Email;
            var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link), CultureInfo.CurrentUICulture);
            await _queueProvider.InsertMessageAsync(message, token);
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendEmailAsync(
            ReturnUrlRequest returnUrl,
            CancellationToken token)
        {
            var data = TempData.Peek(EmailTime);
            if (data != null)
            {
                var temp = DateTime.Parse(TempData.Peek(EmailTime).ToString(), CultureInfo.InvariantCulture);

                if (temp > DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(0.5)))
                {
                    return Ok();
                }
                
            }

            var email = TempData.Peek(Email); //?? throw new ArgumentNullException("TempData", "email is empty");
            if (email == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["EmailResend"]);
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(email.ToString());
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["UserNotExists"]);
                return BadRequest(ModelState);
            }

            TempData[EmailTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            await GenerateEmailAsync(user, returnUrl, token);
            return Ok();
        }
    }
}