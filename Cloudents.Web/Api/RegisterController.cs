﻿using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Cloudents.Web.Controllers;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class RegisterController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IQueueProvider _queueProvider;
        private readonly IStringLocalizer<RegisterController> _localizer;
        private readonly ILogger _logger;
        private readonly ICommandBus _commandBus;
        private readonly ICountryService _countryProvider;

        private const string Email = "email2";
        private const string EmailTime = "EmailTime";

        public RegisterController(UserManager<User> userManager, SignInManager<User> signInManager,
             IQueueProvider queueProvider, IStringLocalizer<RegisterController> localizer,
             ILogger logger, ICountryService countryProvider, ICommandBus commandBus)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _queueProvider = queueProvider;
            _localizer = localizer;
            _logger = logger;
            _countryProvider = countryProvider;
            _commandBus = commandBus;
        }

        [HttpPost, ValidateRecaptcha("6LfyBqwUAAAAALL7JiC0-0W_uWX1OZvBY4QS_OfL"), ValidateEmail]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ReturnSignUserResponse>> PostAsync(
            [FromBody] RegisterRequest model,
            [FromHeader(Name = "User-Agent")] string? userAgent,
            CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                ModelState.AddModelError(nameof(model.Email), _localizer["UserExists"]);
                return BadRequest(ModelState);
            }

            var countryCode = await _countryProvider.GetUserCountryAsync(token);
            user = new User(model.Email, model.FirstName, model.LastName,
                CultureInfo.CurrentCulture, countryCode, model.UserType == UserType.Tutor);
            var p = await _userManager.CreateAsync(user, model.Password);

            if (p.Succeeded)
            {
                var t1 =  FinishRegistrationAsync(user, userAgent, token);
                var t2 = GenerateEmailAsync(user, token);
                await Task.WhenAll(t1, t2);
                return ReturnSignUserResponse.SignIn();
            }


            ModelState.AddIdentityModelError(p);
            return BadRequest(ModelState);
        }





        private async Task GenerateEmailAsync(User user, CancellationToken token)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = System.Net.WebUtility.UrlEncode(code);

            TempData[EmailTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            var link = Url.Link("ConfirmEmail", new
            {
                user.Id,
                code = encodedCode,
                referral = TempData[HomeController.Referral]
            });
            _logger.Info("generate Email", new Dictionary<string, string>()
            {
                ["userId"] = user.Id.ToString(),
                ["code"] = code,
                ["encoded"] = encodedCode,
                ["link"] = link
            });
            TempData[Email] = user.Email;
            var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link), CultureInfo.CurrentUICulture);
            await _queueProvider.InsertMessageAsync(message, token);
        }

        private async Task FinishRegistrationAsync(User user,
            string userAgent, CancellationToken token)
        {
            if (TempData[HomeController.Referral] != null)
            {
                if (Base62.TryParse(TempData[HomeController.Referral]?.ToString(), out var base62))
                {
                    try
                    {
                        var command = new ReferringUserCommand(base62, user.Id);
                        await _commandBus.DispatchAsync(command, token);
                    }
                    catch (UserLockoutException)
                    {
                        _logger.Warning($"{user.Id} got locked referring user {TempData[HomeController.Referral]}");
                    }
                }
                else
                {
                    _logger.Error($"{user.Id} got wrong referring user {TempData[HomeController.Referral]}");
                }
                TempData.Remove(HomeController.Referral);
            }
            TempData.Clear();

            var command2 = new AddUserLocationCommand(user, HttpContext.GetIpAddress(), userAgent);
            var t1 = _commandBus.DispatchAsync(command2, token);
            var t2 = _signInManager.SignInAsync(user, false);
            await Task.WhenAll(t1, t2);
           
        }

        [Authorize]
        [HttpPost("verifyEmail")]
        public async Task<IActionResult> TutorVerifyEmail(CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User);
            await GenerateEmailAsync(user, token);
            return Ok();
        }

        [HttpPost("resend")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ResendEmailAsync(
            CancellationToken token)
        {
            var data = TempData.Peek(EmailTime);
            if (data != null)
            {
                var temp = DateTime.Parse(data.ToString()!, CultureInfo.InvariantCulture);

                if (temp > DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(0.5)))
                {
                    return Ok();
                }

            }

            var email = TempData.Peek(Email);
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
            await GenerateEmailAsync(user, token);
            return Ok();
        }
    }
}