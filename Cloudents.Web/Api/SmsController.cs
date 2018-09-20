using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhoneNumbers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Web.Controllers;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]

    public class SmsController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IServiceBusProvider _serviceBus;
        private readonly ISmsSender _client;
        private readonly ICommandBus _commandBus;
        private readonly IStringLocalizer<DataAnnotationSharedResource> _localizer;

        public SmsController(SignInManager<User> signInManager, UserManager<User> userManager, IServiceBusProvider serviceBus,
            ISmsSender client,  ICommandBus commandBus, IStringLocalizer<DataAnnotationSharedResource> localizer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _serviceBus = serviceBus;
            _client = client;
            _commandBus = commandBus;
            _localizer = localizer;
        }

        [HttpGet("code")]
        public async Task<CallingCallResponse> GetCountryCallingCodeAsync([FromServices] IIpToLocation service, CancellationToken token)
        {
            var result = await service.GetAsync(HttpContext.Connection.GetIpAddress(), token).ConfigureAwait(false);
            return new CallingCallResponse(result?.CallingCode);
        }

        [HttpPost]
        public async Task<IActionResult> SetUserPhoneNumber(
            [FromBody]PhoneNumberRequest model/*, LocationQuery location*/,
            CancellationToken token)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                var ex = new ArgumentNullException(nameof(user));
                ex.Data.Add("model", model.ToString());
                throw ex;
            }
            if (!user.EmailConfirmed || user.PhoneNumberConfirmed)
            {
                return Unauthorized();
            }

            var phoneNumber = await _client.ValidateNumberAsync(model.ToString(), token).ConfigureAwait(false);
            if (string.IsNullOrEmpty(phoneNumber))
            {
                ModelState.AddModelError(nameof(model.PhoneNumber), _localizer["InvalidPhoneNumber"]);
                return BadRequest(ModelState);
            }

            //if (ValidatePhoneNumberLocationWithIp(location, model))
            //{
            //    user.FraudScore += 50;
            //}
            var retVal = await _userManager.SetPhoneNumberAsync(user, phoneNumber).ConfigureAwait(false);

            if (retVal.Succeeded)
            {
                var t1 = _serviceBus.InsertMessageAsync(new TalkJsUser(user.Id, user.Name)
                {
                    Email = user.Email
                }, token);
                //var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
                var t3 = _client.SendSmsAsync(user, token);
                await Task.WhenAll(t1, t3).ConfigureAwait(false);
                return Ok();
            }

            if (retVal.Errors.Any(a => a.Code == "Duplicate"))
            {
                //TODO: Localize
                ModelState.AddModelError(string.Empty, "This phone number is linked to another email address");
            }
            else
            {
                ModelState.AddIdentityModelError(retVal);
            }

            return BadRequest(ModelState);
        }

        private static bool ValidatePhoneNumberLocationWithIp(LocationQuery location, PhoneNumberRequest phoneNumber)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();
            var t = phoneUtil.GetRegionCodeForCountryCode(phoneNumber.CountryCode);
            return t.Equals(location.Address.CountryCode, StringComparison.OrdinalIgnoreCase);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifySmsAsync([FromBody]CodeRequest model, CancellationToken token)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                var ex = new ArgumentNullException(nameof(user));
                ex.Data.Add("model", model.ToString());
                throw ex;
            }

            var v = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, model.Number).ConfigureAwait(false);

            if (v.Succeeded)
            {
                //This is the last step of the registration.
                return await FinishRegistrationAsync(token, user);
            }
            ModelState.AddIdentityModelError(v);
            return BadRequest(ModelState);
        }

        private async Task<IActionResult> FinishRegistrationAsync(CancellationToken token, User user)
        {
            if (TempData[HomeController.Referral] != null)
            {
                var base62 = new Base62(TempData[HomeController.Referral].ToString());
                var command = new DistributeTokensCommand(base62.Value, 10, ActionType.ReferringUser);
                await _commandBus.DispatchAsync(command, token);
                TempData.Remove(HomeController.Referral);
            }
            TempData.Clear();
            await _signInManager.SignInAsync(user, false).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendAsync(CancellationToken token)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                //TODO: Localize
                ModelState.AddModelError(string.Empty, "We cannot resend sms");
                return BadRequest(ModelState);
            }

            await _client.SendSmsAsync(user, token).ConfigureAwait(false);
            return Ok();
        }
    }
}