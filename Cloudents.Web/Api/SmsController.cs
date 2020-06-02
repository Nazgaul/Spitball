using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Controllers;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Infrastructure;
using Cloudents.Query;
using Microsoft.AspNetCore.Authorization;
using SbUserManager = Cloudents.Web.Identity.SbUserManager;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]

    public class SmsController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly SbUserManager _userManager;
        private readonly ISmsSender _client;
        private readonly ICommandBus _commandBus;
        //private readonly IStringLocalizer<DataAnnotationSharedResource> _localizer;
        private readonly IStringLocalizer<SmsController> _smsLocalizer;
        private readonly ILogger _logger;

        private const string SmsTime = "SmsTime";
        private const string PhoneCallTime = "phoneCallTime";

        public SmsController(SignInManager<User> signInManager, SbUserManager userManager,
            ISmsSender client, ICommandBus commandBus,
            //IStringLocalizer<DataAnnotationSharedResource> localizer,
            ILogger logger, IStringLocalizer<SmsController> smsLocalizer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _client = client;
            _commandBus = commandBus;
            // _localizer = localizer;
            _logger = logger;
            _smsLocalizer = smsLocalizer;
        }

        [HttpGet("code")]
        [ResponseCache(Duration = TimeConst.Hour, Location = ResponseCacheLocation.Client)]

        public async Task<CallingCallResponse> GetCountryCallingCodeAsync(
            [FromServices] IQueryBus service, CancellationToken token)
        {
            var query = new CountryByIpQuery(HttpContext.GetIpAddress().ToString());
            var result = await service.QueryAsync(query, token);
            return new CallingCallResponse(result?.CallingCode, result?.CountryCode);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> SetUserPhoneNumberAsync(
            [FromBody] PhoneNumberRequest model,
            [FromHeader(Name = "User-Agent")] string? agent,
            CancellationToken token)
        {
            User user;
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.GetUserAsync(User);
                if (user.Tutor == null)
                {
                    return Unauthorized();
                }
            }
            else
            {
                user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            }

            if (user == null)
            {
                _logger.Error("Set User Phone number We can't identify the user");
                return Unauthorized();
            }
            if (user.PhoneNumberConfirmed)
            {
                return Unauthorized();
            }

            var retVal = await _userManager.SetPhoneNumberAndCountryAsync(user,
                model.PhoneNumber, model.CountryCode.ToString(), token);

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
                if (User.Identity.IsAuthenticated)
                {
                    TempData[SmsTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                    TempData[PhoneCallTime] = DateTime.UtcNow.AddMinutes(-2).ToString(CultureInfo.InvariantCulture);
                    await _client.SendSmsAsync(user, token);
                    return Ok();
                }
            
                return await FinishRegistrationAsync(user, agent, token);
            }

            if (retVal.Errors.Any(a => a.Code == "CountryNotSupported"))
            {
                //var command2 = new AddUserLocationCommand(user, country, HttpContext.Connection.GetIpAddress());
                //var t1 = _commandBus.DispatchAsync(command2, token);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                ModelState.AddModelError(nameof(model.PhoneNumber), _smsLocalizer["CountryNotSupported"]);
                var t2 = _signInManager.SignOutAsync();
                await Task.WhenAll(t2);
                return BadRequest(ModelState);
            }
            if (retVal.Errors.Any(a => a.Code == "InvalidPhoneNumber"))
            {
                ModelState.AddModelError(nameof(model.PhoneNumber), "InvalidPhoneNumber");
            }
            if (retVal.Errors.Any(a => a.Code == "Duplicate"))
            {
                _logger.Warning("phone number is duplicate");
                ModelState.AddModelError(nameof(model.PhoneNumber), "DuplicatePhoneNumber");
            }
            else
            {
                _logger.Warning("Some other error" + retVal.Errors.FirstOrDefault()?.Description);
                ModelState.AddIdentityModelError(retVal);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost("sendCode")]
        public async Task<IActionResult> SendVerificationCodeAsync(CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.PhoneNumberConfirmed)
            {
                return BadRequest();
            }

            if (user.Tutor == null)
            {
                return BadRequest();

            }
            await _client.SendSmsAsync(user, token);
            return Ok();
        }


        [HttpPost("verify")]
        [Authorize]
        public async Task<IActionResult> VerifySmsAsync(
            [FromBody] CodeRequest model,
            CancellationToken token)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.Error("VerifySmsAsync We can't identify the user");
                return Unauthorized();
            }
            token.ThrowIfCancellationRequested();
            var v = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, model.Number);
            if (v.Succeeded)
            {
                return Ok();
            }
            _logger.Warning($"userid: {user.Id} is not verified reason: {v}");
            ModelState.AddIdentityModelError(v);
            return BadRequest(ModelState);
        }

        private async Task<IActionResult> FinishRegistrationAsync(User user,
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

        [HttpPost("resend"),Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ResendAsync(CancellationToken token)
        {
            var t = TempData.Peek(SmsTime);
            if (t == null)
            {
                TempData[SmsTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                return Ok();
            }

            var temp = DateTime.Parse(t.ToString(), CultureInfo.InvariantCulture);
            if (temp > DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(0.5)))
            {
                return Ok();
            }

            var user = await _userManager.GetUserAsync(User);
            TempData[SmsTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            await _client.SendSmsAsync(user, token);
            return Ok();
        }

        [HttpPost("call"),Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CallUserAsync(CancellationToken token)
        {
            var t = TempData.Peek(PhoneCallTime);
            if (t == null)
            {
                TempData[PhoneCallTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                return Ok();
            }
            var user = await _userManager.GetUserAsync(User);
            TempData[PhoneCallTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            await _client.SendPhoneAsync(user, token);
            return Ok();
        }
    }
}