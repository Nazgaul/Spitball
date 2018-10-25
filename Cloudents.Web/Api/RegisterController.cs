using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
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

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class RegisterController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SbSignInManager _signInManager;

        private readonly IBlockChainErc20Service _blockChainErc20Service;
        private readonly IServiceBusProvider _queueProvider;
        private readonly ISmsSender _client;
        private readonly IStringLocalizer<RegisterController> _localizer;
        private readonly IStringLocalizer<LogInController> _loginLocalizer;

        internal const string Email = "email2";


        public RegisterController(UserManager<User> userManager, SbSignInManager signInManager, IBlockChainErc20Service blockChainErc20Service, IServiceBusProvider queueProvider, ISmsSender client, IStringLocalizer<RegisterController> localizer, IStringLocalizer<LogInController> loginLocalizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _blockChainErc20Service = blockChainErc20Service;
            _queueProvider = queueProvider;
            _client = client;
            _localizer = localizer;
            _loginLocalizer = loginLocalizer;
        }

        [HttpPost, ValidateRecaptcha, ValidateEmail]
        public async Task<ActionResult<ReturnSignUserResponse>> Post(
            [FromBody]RegisterRequest model,
            [CanBeNull] ReturnUrlRequest returnUrl,
            CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    await GenerateEmailAsync(user, returnUrl, token).ConfigureAwait(false);
                    return new ReturnSignUserResponse(NextStep.EmailConfirmed, false);
                }

                if (string.IsNullOrEmpty(user.PhoneNumber))
                {
                    await _signInManager.TempSignIn(user).ConfigureAwait(false);
                    return new ReturnSignUserResponse(NextStep.EnterPhone, false);

                }
                if (!user.PhoneNumberConfirmed)
                {
                    var t1 = _signInManager.TempSignIn(user);
                    var t2 = _client.SendSmsAsync(user, token);

                    await Task.WhenAll(t1, t2);
                    return new ReturnSignUserResponse(NextStep.VerifyPhone, false);
                }
                ModelState.AddModelError(nameof(model.Email), _localizer["UserExists"]);
                return BadRequest(ModelState);
            }
            user = CreateUser(model.Email, null);
            var p = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
            if (p.Succeeded)
            {
                await GenerateEmailAsync(user, returnUrl, token).ConfigureAwait(false);
                return new ReturnSignUserResponse(NextStep.EmailConfirmed, true);
            }
            ModelState.AddIdentityModelError(p);
            return BadRequest(ModelState);
        }

        



        [HttpPost("google")]
        public async Task<ActionResult<ReturnSignUserResponse>> GoogleSignInAsync([FromBody] GoogleTokenRequest model,
            ReturnUrlRequest returnUrl,
            [FromServices] IGoogleAuth service,
            CancellationToken cancellationToken)
        {

            var result = await service.LogInAsync(model.Token, cancellationToken).ConfigureAwait(false);
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

                var result3 = await _userManager.CreateAsync(user);

                if (result3.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", result.Id, result.Name));
                    await _signInManager.TempSignIn(user).ConfigureAwait(false);
                    return new ReturnSignUserResponse(NextStep.EnterPhone, true);
                }
            }
            else
            {
                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                }
                await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", result.Id, result.Name));
                if (user.PhoneNumberConfirmed)
                {
                    await _signInManager.SignInAsync(user, false);
                    return new ReturnSignUserResponse(false);
                }
                await _signInManager.TempSignIn(user).ConfigureAwait(false);
                return new ReturnSignUserResponse(NextStep.EnterPhone, true);
            }

            ModelState.AddModelError("Google", _localizer["GoogleUserRegisteredWithEmail"]);
            return BadRequest(ModelState);
        }


        private User CreateUser(string email, string name)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(name))
            {
                name = email.Split(new[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            var (privateKey, _) = _blockChainErc20Service.CreateAccount();
            return new User(email, $"{name}.{GenerateRandomNumber()}", privateKey, CultureInfo.CurrentCulture);
        }

        private static int GenerateRandomNumber()
        {
            var rdm = new Random();
            return rdm.Next(1000, 9999);
        }

        private async Task GenerateEmailAsync(User user, [CanBeNull] ReturnUrlRequest returnUrl, CancellationToken token)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
            code = UrlEncoder.Default.Encode(code);
            var url = returnUrl?.Url;
            if (!Url.IsLocalUrl(url))
            {
                url = null;
            }

            var link = Url.Link("ConfirmEmail", new { user.Id, code, returnUrl = url, referral = TempData[HomeController.Referral] });
            TempData[Email] = user.Email;
            var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link),CultureInfo.CurrentUICulture);
            await _queueProvider.InsertMessageAsync(message, token).ConfigureAwait(false);
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendEmailAsync(
            ReturnUrlRequest returnUrl,
            CancellationToken token)
        {
            var email = TempData.Peek(Email); //?? throw new ArgumentNullException("TempData", "email is empty");
            if (email == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["EmailResend"]);
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(email.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["UserNotExists"]);
                return BadRequest(ModelState);
            }

            await GenerateEmailAsync(user, returnUrl, token).ConfigureAwait(false);
            return Ok();
        }
    }
}