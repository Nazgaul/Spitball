using Cloudents.Core;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]

    public class SmsController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ISmsSender _client;
        private readonly ICommandBus _commandBus;
        private readonly IStringLocalizer<DataAnnotationSharedResource> _localizer;
        private readonly IStringLocalizer<SmsController> _smsLocalizer;
        private readonly ILogger _logger;


        public SmsController(SignInManager<User> signInManager, UserManager<User> userManager,
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
        public async Task<CallingCallResponse> GetCountryCallingCodeAsync(

            [FromServices] IIpToLocation service, CancellationToken token)
        {
            var result = await service.GetAsync(HttpContext.Connection.GetIpAddress(), token).ConfigureAwait(false);
            return new CallingCallResponse(result?.CallingCode);
        }

        [HttpPost]
        public async Task<IActionResult> SetUserPhoneNumber(
            [ModelBinder(typeof(CountryModelBinder))] string country,
            [FromBody]PhoneNumberRequest model,
            CancellationToken token)
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.Error("Set User Phone number User is already sign in");
                return Unauthorized();
            }
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                _logger.Error("Set User Phone number We can't identify the user");
                return Unauthorized();
            }
            if (!user.EmailConfirmed || user.PhoneNumberConfirmed)
            {
                return Unauthorized();
            }

            var phoneNumber = await _client.ValidateNumberAsync(model.ToString(), token).ConfigureAwait(false);
            if (string.IsNullOrEmpty(phoneNumber))
            {
                _logger.Warning("Did not passed validation of lookup");
                ModelState.AddModelError(nameof(model.PhoneNumber), _localizer["InvalidPhoneNumber"]);
                return BadRequest(ModelState);
            }


            var phoneUtil = PhoneNumberUtil.GetInstance();
            var t = phoneUtil.GetRegionCodeForCountryCode(model.CountryCode);
            user.Country = t;

            var retVal = await _userManager.SetPhoneNumberAsync(user, phoneNumber).ConfigureAwait(false);

            if (!string.Equals(user.Country, country, StringComparison.OrdinalIgnoreCase))
            {
                var command2 = new AddUserLocationCommand(user, country, HttpContext.Connection.GetIpAddress());
                var t1 = _commandBus.DispatchAsync(command2, token);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                ModelState.AddModelError(nameof(model.PhoneNumber), _smsLocalizer["PhoneNumberNotSameCountry"]);
                var t2 =  _signInManager.SignOutAsync();
                await Task.WhenAll(t1, t2);
                return BadRequest(ModelState);

            }
            if (retVal.Succeeded)
            {
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
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                _logger.Error("VerifySmsAsync We can't identify the user");
                return Unauthorized();
                //var ex = new ArgumentNullException(nameof(user));
                //ex.Data.Add("model", model.ToString());
                //throw ex;
            }

            var v = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, model.Number).ConfigureAwait(false);

            if (v.Succeeded)
            {
                //This is the last step of the registration.
                return await FinishRegistrationAsync(token, user, country);
            }
            ModelState.AddIdentityModelError(v);
            return BadRequest(ModelState);
        }

        private async Task<IActionResult> FinishRegistrationAsync(CancellationToken token, User user, string country)
        {
            if (TempData[HomeController.Referral] != null)
            {
                if (Base62.TryParse(TempData[HomeController.Referral].ToString(), out var base62))
                {
                    var command = new ReferringUserCommand(base62.Value, user.Id);
                    await _commandBus.DispatchAsync(command, token);
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
            var t2 =  _signInManager.SignInAsync(user, false);
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
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, _smsLocalizer["CannotResendSms"]);
                return BadRequest(ModelState);
            }

            await _client.SendSmsAsync(user, token).ConfigureAwait(false);
            return Ok();
        }
    }
}