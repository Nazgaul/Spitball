﻿using System;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class SignUserController : Controller
    {
        private readonly UserManager<User> _userManager;
        //private readonly SbSignInManager _signInManager;
        private readonly ISmsSender _smsClient;
        private readonly IBlockChainErc20Service _blockChainErc20Service;
        private readonly IServiceBusProvider _queueProvider;
        internal const string Email = "email";



        public SignUserController(UserManager<User> userManager, /*SbSignInManager signInManager,*/ ISmsSender smsClient,
            IBlockChainErc20Service blockChainErc20Service, IServiceBusProvider queueProvider
            )
        {
            _userManager = userManager;
            //_signInManager = signInManager;
            _smsClient = smsClient;
            _blockChainErc20Service = blockChainErc20Service;
            _queueProvider = queueProvider;
        }



        //[HttpPost, ValidateRecaptcha, ValidateEmail]
        //public async Task<ActionResult<ReturnSignUserResponse>> SignUser([FromBody]SignUserRequest model,
        //   [CanBeNull] ReturnUrlRequest returnUrl, CancellationToken token)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        ModelState.AddModelError(string.Empty, "user is already logged in");
        //        return BadRequest(ModelState);
        //    }

        //    var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        //    if (user == null)
        //    {
        //        user = CreateUser(model.Email, model.Name);
        //        user.EmailConfirmed = model.EmailConfirmed;

        //        var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
        //        if (p.Succeeded)
        //        {
        //            if (!user.EmailConfirmed)
        //            {
        //                await GenerateEmailAsync(user, returnUrl, token).ConfigureAwait(false);
        //                return new ReturnSignUserResponse(NextStep.EmailConfirmed, true);

        //            }
        //            await _signInManager.SignInTwoFactorAsync(user, false).ConfigureAwait(false);
        //            return new ReturnSignUserResponse(NextStep.EnterPhone, true);

        //        }
        //        ModelState.AddIdentityModelError(p);
        //        return BadRequest(ModelState);
        //    }
        //    if (!user.EmailConfirmed)
        //    {
        //        await GenerateEmailAsync(user, returnUrl, token).ConfigureAwait(false);
        //        return new ReturnSignUserResponse(NextStep.EmailConfirmed, false);
        //    }

        //    if (!user.PhoneNumberConfirmed)
        //    {
        //        await _signInManager.SignInTwoFactorAsync(user, false).ConfigureAwait(false);
        //        return new ReturnSignUserResponse(NextStep.EnterPhone, false);
        //    }
        //    var taskSignIn = _signInManager.SignInTwoFactorAsync(user, false);
        //    var taskSms = _smsClient.SendSmsAsync(user, token);


        //    TempData["SMS"] = user.Email;
        //    await Task.WhenAll(taskSms, taskSignIn).ConfigureAwait(false);

        //    if (taskSignIn.Result.RequiresTwoFactor)
        //    {
        //        return new ReturnSignUserResponse(NextStep.VerifyPhone, false);
        //    }
        //    ModelState.AddModelError(string.Empty, taskSignIn.Result.ToString());
        //    return BadRequest(ModelState);
        //}


        private async Task GenerateEmailAsync(User user,[CanBeNull] ReturnUrlRequest returnUrl, CancellationToken token)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
            code = UrlEncoder.Default.Encode(code);
            var url = returnUrl?.Url;
            if (!Url.IsLocalUrl(url))
            {
                url = null;
            }
            var link = Url.Link("ConfirmEmail", new { user.Id, code, returnUrl =  url });
            TempData[Email] = user.Email;
            var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link));
            await _queueProvider.InsertMessageAsync(message, token).ConfigureAwait(false);
        }

        //[HttpPost("google")]
        //public async Task<ActionResult<ReturnSignUserResponse>> GoogleSignInAsync([FromBody] GoogleTokenRequest model,
        //    ReturnUrlRequest returnUrl,
        //    [FromServices] IGoogleAuth service,
        //    CancellationToken cancellationToken)
        //{
        //    var result = await service.LogInAsync(model.Token, cancellationToken).ConfigureAwait(false);
        //    if (result == null)
        //    {
        //        ModelState.AddModelError(string.Empty, "No result from google");
        //        return BadRequest(ModelState);
        //    }

        //    return await SignUser(new SignUserRequest(result.Email, true, result.Name), returnUrl, cancellationToken).ConfigureAwait(false);
        //}

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