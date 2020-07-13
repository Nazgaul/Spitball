using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
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
    [Authorize(Policy = "Tutor")]

    public class SmsController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly SbUserManager _userManager;
        private readonly ISmsSender _client;
        private readonly IStringLocalizer<SmsController> _smsLocalizer;
        private readonly ILogger _logger;

        private const string SmsTime = "SmsTime";
        private const string PhoneCallTime = "phoneCallTime";

        public SmsController(SignInManager<User> signInManager, SbUserManager userManager,
            ISmsSender client, 
            ILogger logger, IStringLocalizer<SmsController> smsLocalizer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _client = client;
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
            CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.PhoneNumberConfirmed)
            {
                _logger.Error("Phone number is confirmed Unauthorized");
                return Unauthorized();
            }

            var retVal = await _userManager.SetPhoneNumberAndCountryAsync(user,
                model.PhoneNumber, model.CountryCode.ToString(), token);

            if (retVal.Succeeded)
            {

                TempData[SmsTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                TempData[PhoneCallTime] = DateTime.UtcNow.AddMinutes(-2).ToString(CultureInfo.InvariantCulture);
                await _client.SendSmsAsync(user, token);
                return Ok();

            }

            if (retVal.Errors.Any(a => a.Code == "CountryNotSupported"))
            {
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
            return Ok(new
            {
                phoneNumber = user.PhoneNumber
            });
        }


        [HttpPost("verify")]
       
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


        [HttpPost("resend")]
        
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

        [HttpPost("call")]
       
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CallUserAsync(CancellationToken token)
        {
            var t = TempData.Peek(PhoneCallTime);
            var temp = DateTime.UtcNow.AddDays(-1);
            if (t != null)
            {
                temp = DateTime.Parse(t.ToString(), CultureInfo.InvariantCulture);
            }
            if (temp > DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(0.5)))
            {
                return Ok();
            }
            var user = await _userManager.GetUserAsync(User);
            TempData[PhoneCallTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            await _client.SendPhoneAsync(user, token);
            return Ok();
        }
    }
}