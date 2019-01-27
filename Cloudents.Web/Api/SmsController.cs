﻿using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Binders;
using Cloudents.Web.Controllers;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PhoneNumbers;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]

    public class SmsController : Controller
    {
        private readonly SignInManager<RegularUser> _signInManager;
        private readonly UserManager<RegularUser> _userManager;
        private readonly ISmsSender _client;
        private readonly ICommandBus _commandBus;
        private readonly IStringLocalizer<DataAnnotationSharedResource> _localizer;
        private readonly IStringLocalizer<SmsController> _smsLocalizer;
        private readonly ILogger _logger;

        private const string SmsTime = "SmsTime";

        public SmsController(SignInManager<RegularUser> signInManager, UserManager<RegularUser> userManager,
            ISmsSender client, ICommandBus commandBus, IStringLocalizer<DataAnnotationSharedResource> localizer,
            ILogger logger, IStringLocalizer<SmsController> smsLocalizer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _client = client;
            _commandBus = commandBus;
            _localizer = localizer;
            _logger = logger;
            _smsLocalizer = smsLocalizer;
        }

        [HttpGet("code")]
        [ResponseCache(Duration = TimeConst.Hour, Location = ResponseCacheLocation.Client)]

        public async Task<CallingCallResponse> GetCountryCallingCodeAsync(

            [FromServices] IIpToLocation service, CancellationToken token)
        {
            var result = await service.GetAsync(HttpContext.Connection.GetIpAddress(), token);
            return new CallingCallResponse(result?.CallingCode);
        }

        [HttpPost]
        public async Task<IActionResult> SetUserPhoneNumber(
           // [ModelBinder(typeof(CountryModelBinder))] string country,
            [FromBody]PhoneNumberRequest model,
            CancellationToken token)
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.Error("Set User Phone number User is already sign in");
                return Unauthorized();
            }
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.Error("Set User Phone number We can't identify the user");
                return Unauthorized();
            }
            if (!user.EmailConfirmed || user.PhoneNumberConfirmed)
            {
                return Unauthorized();
            }

            var phoneNumber = await _client.ValidateNumberAsync(model.ToString(), token);
            if (string.IsNullOrEmpty(phoneNumber))
            {
                _logger.Warning("Did not passed validation of lookup");
                ModelState.AddModelError(nameof(model.PhoneNumber), _localizer["InvalidPhoneNumber"]);
                return BadRequest(ModelState);
            }


            var phoneUtil = PhoneNumberUtil.GetInstance();
            var t = phoneUtil.GetRegionCodeForCountryCode(model.CountryCode);
            user.Country = t;

            var retVal = await _userManager.SetPhoneNumberAsync(user, phoneNumber);

            //Ram: I disable this - we have an issue that sometime we get the wrong ip look at id 
            //3DCDBF98-6545-473A-8EAA-A9DF00787C70 of UserLocation table in dev sql
            //if (country != null)
            //{
            //    if (!string.Equals(user.Country, country, StringComparison.OrdinalIgnoreCase))
            //    {
            //        var command2 = new AddUserLocationCommand(user, country, HttpContext.Connection.GetIpAddress());
            //        var t1 = _commandBus.DispatchAsync(command2, token);
            //        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            //        ModelState.AddModelError(nameof(model.PhoneNumber), _smsLocalizer["PhoneNumberNotSameCountry"]);
            //        var t2 = _signInManager.SignOutAsync();
            //        await Task.WhenAll(t1, t2);
            //        return BadRequest(ModelState);

            //    }
            //}

            if (retVal.Succeeded)
            {
                TempData[SmsTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                await _client.SendSmsAsync(user, token);
                return Ok();
            }

            if (retVal.Errors.Any(a => a.Code == "Duplicate"))
            {
                _logger.Warning("phone number is duplicate");
                ModelState.AddModelError(nameof(model.PhoneNumber), _smsLocalizer["DuplicatePhoneNumber"]);
            }
            else
            {
                _logger.Warning("Some other error" + retVal.Errors.FirstOrDefault()?.Description);
                ModelState.AddIdentityModelError(retVal);
            }

            return BadRequest(ModelState);
        }


        [HttpPost("verify")]
        public async Task<IActionResult> VerifySmsAsync(
            [FromBody]CodeRequest model,
            [ModelBinder(typeof(CountryModelBinder))] string country,
            CancellationToken token)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.Error("VerifySmsAsync We can't identify the user");
                return Unauthorized();
            }

            var v = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, model.Number);
            if (v.Succeeded)
            {
                //This is the last step of the registration.

                return await FinishRegistrationAsync(token, user, country);
            }
            _logger.Warning($"userid: {user.Id} is not verified reason: {v}");
            ModelState.AddIdentityModelError(v);
            return BadRequest(ModelState);
        }

        private async Task<IActionResult> FinishRegistrationAsync(CancellationToken token, RegularUser user, string country)
        {
            if (TempData[HomeController.Referral] != null)
            {
                if (Base62.TryParse(TempData[HomeController.Referral].ToString(), out var base62))
                {
                    try
                    {
                        var command = new ReferringUserCommand(base62.Value, user.Id);
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

            var command2 = new AddUserLocationCommand(user, country, HttpContext.Connection.GetIpAddress());
            var registrationBonusCommand = new FinishRegistrationCommand(user.Id);
            var t1 = _commandBus.DispatchAsync(command2, token);
            var t2 = _signInManager.SignInAsync(user, false);
            var t3 = _commandBus.DispatchAsync(registrationBonusCommand, token);
            await Task.WhenAll(t1, t2, t3);
            return Ok(new
            {
                user.Id
            });
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendAsync(CancellationToken token)
        {
            var t = TempData.Peek(SmsTime);
            if (t != null)
            {
                var temp = DateTime.Parse(t.ToString(), CultureInfo.InvariantCulture);
                if (temp > DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(0.5)))
                {
                    return Ok();
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                _logger.Error("Set User Phone number User is already sign in");
                return Unauthorized();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, _smsLocalizer["CannotResendSms"]);
                return BadRequest(ModelState);
            }

            TempData[SmsTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            await _client.SendSmsAsync(user, token);
            return Ok();
        }
    }
}