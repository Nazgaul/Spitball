﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"),ApiController]

    public class SmsController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IServiceBusProvider _serviceBus;
        private readonly ISmsSender _client;

        public SmsController(SignInManager<User> signInManager, UserManager<User> userManager, IServiceBusProvider serviceBus,
            ISmsSender client)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _serviceBus = serviceBus;
            _client = client;

        }

        [HttpGet("code")]
        public async Task<CallingCallResponse> GetCountryCallingCodeAsync([FromServices] IIpToLocation service, CancellationToken token)
        {
            var result = await service.GetAsync(HttpContext.Connection.GetIpAddress(), token).ConfigureAwait(false);
            return new CallingCallResponse(result?.CallingCode);
        }

        [HttpPost]
        public async Task<IActionResult> SmsUserAsync(
            [FromBody]PhoneNumberRequest model,
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

            var phoneNumber = await _client.ValidateNumberAsync(model.Number, token).ConfigureAwait(false);
            if (string.IsNullOrEmpty(phoneNumber))
            {
                ModelState.AddModelError(string.Empty, "Invalid phone number");
                return BadRequest(ModelState);
            }

            var retVal = await _userManager.SetPhoneNumberAsync(user, phoneNumber).ConfigureAwait(false);

            if (retVal.Succeeded)
            {
                var t1 = _serviceBus.InsertMessageAsync(new TalkJsUser(user.Id, user.Name)
                {
                    Email = user.Email
                }, token);
                var t3 = _client.SendSmsAsync(user, token);
                await Task.WhenAll(t1, t3).ConfigureAwait(false);
                return Ok();
            }

            if (retVal.Errors.Any(a => a.Code == "Duplicate"))
            {
                ModelState.AddModelError(string.Empty, "This phone number is linked to another email address");
            }
            else
            {
                ModelState.AddIdentityModelError(retVal);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifySmsAsync([FromBody]CodeRequest model)
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
                await _signInManager.SignInAsync(user, false).ConfigureAwait(false);
                return Ok();
            }
            ModelState.AddIdentityModelError(v);
            return BadRequest(ModelState);
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendAsync(CancellationToken token)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "We cannot resend email");
                return BadRequest(ModelState);
            }

            await _client.SendSmsAsync(user, token).ConfigureAwait(false);
            return Ok();
        }
    }
}