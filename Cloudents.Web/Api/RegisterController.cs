using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Web.Filters;
using JetBrains.Annotations;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class RegisterController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IBlockChainErc20Service _blockChainErc20Service;
        private readonly IServiceBusProvider _queueProvider;
        internal const string Email = "email2";


        public RegisterController(UserManager<User> userManager, SignInManager<User> signInManager, IBlockChainErc20Service blockChainErc20Service, IServiceBusProvider queueProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _blockChainErc20Service = blockChainErc20Service;
            _queueProvider = queueProvider;
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
                //if (!user.PhoneNumberConfirmed)
                //{
                //    await _signInManager.SignInTwoFactorAsync(user, false).ConfigureAwait(false);
                //    return new ReturnSignUserResponse(NextStep.EnterPhone, false);
                //}

                ModelState.AddModelError("user already exists");
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
                ModelState.AddModelError(string.Empty, "No result from google");
                return BadRequest(ModelState);
            }
            
           // var user = await _userManager.FindByEmailAsync(result.Email).ConfigureAwait(false);
            //if (user == null)
           // {
                //ModelState.AddModelError("user already exists");
                //return BadRequest(ModelState);
            //}
            var result2 = await _signInManager.ExternalLoginSignInAsync("Google", result.Id, false);
            if (result2.Succeeded)
            {
                // Update any authentication tokens if login succeeded
                return Ok();
            }
            //if (result2.RequiresTwoFactor)
            //{
            //    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            //}
            if (result2.IsLockedOut)
            {
                return BadRequest();
                
            }

            var user = CreateUser(result.Email, result.Name);
            var result3 = await _userManager.CreateAsync(user);

            if (result3.Succeeded)
            {
                await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", result.Id, result.Name));
                //await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
            }
            //var result2 = await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", result.Id, result.Name));
            return Ok();
        }


        private User CreateUser(string email, string name)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(name))
            {
                name = email.Split(new[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            var (privateKey, _) = _blockChainErc20Service.CreateAccount();
            return new User(email, $"{name}.{GenerateRandomNumber()}", privateKey);
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
            var link = Url.Link("ConfirmEmail", new { user.Id, code, returnUrl = url });
            TempData[Email] = user.Email;
            var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link));
            await _queueProvider.InsertMessageAsync(message, token).ConfigureAwait(false);
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendEmailAsync(
            ReturnUrlRequest returnUrl,
            CancellationToken token)
        {
            var email = TempData.Peek(Email) ?? throw new ArgumentNullException("TempData", "email is empty");
            var user = await _userManager.FindByEmailAsync(email.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "no user");
                return BadRequest(ModelState);
            }

            await GenerateEmailAsync(user, returnUrl, token).ConfigureAwait(false);
            return Ok();
        }
    }
}