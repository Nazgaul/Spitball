using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfigurationKeys _configuration;
        private readonly IQueueProvider _queueProvider;
        private readonly SignInManager<User> _signInManager;

        public RegisterController(
            UserManager<User> userManager, IConfigurationKeys configuration, IQueueProvider queueProvider, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _queueProvider = queueProvider;
            _signInManager = signInManager;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateUserAsync([FromBody]RegisterEmailRequest model, CancellationToken token)
        {
            var user = new User
            {
                Email = model.Email,
                Name = model.Email // TODO: randomize user name
            };

            var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
            if (p.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                var link = Url.Link("ConfirmEmail", new { user.Id, code });

                var message = new EmailMessage
                {
                    To = model.Email,
                    PlaceHolders = new object[] { HtmlEncoder.Default.Encode(link) },
                    Template = "register",
                    Subject = "welcome to spitball"
                };
                var t1 =  _queueProvider.InsertMessageAsync(message, token);
                var t2 =  _signInManager.SignInAsync(user, isPersistent: false);
                await Task.WhenAll(t1, t2).ConfigureAwait(false);
                return Ok(model.Captcha);
            }

            //await _signInManager.SignInAsync(user, false);
            return BadRequest(p.Errors);
        }

        [HttpPost("google"), ValidateModel]
        public async Task<IActionResult> GoogleSignInAsync([FromBody] TokenRequest model,
            [FromServices] IGoogleAuth service,
            CancellationToken cancellationToken)
        {

            var result = await service.LogInAsync(model.Token, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return BadRequest();
            }

            var user = new User
            {
                Email = result.Email,
                Name = result.Name,
                EmailConfirmed = true
            };
            var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
            if (p.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                return Ok();
            }
            return BadRequest(p.Errors);
        }

        [HttpPost("sms"), ValidateModel]
        [Authorize(Policy = SignInStep.PolicyEmail)]
        public async Task<IActionResult> SmsUserAsync([FromBody]PhoneNumberRequest model, [FromServices] IRestClient client, CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            await _userManager.SetPhoneNumberAsync(user, model.Number).ConfigureAwait(false);
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.Number).ConfigureAwait(false);

            var message = new SmsMessage
            {
                PhoneNumber = model.Number,
                Message = code
            };
            var result = await client.PostJsonAsync(new Uri($"{_configuration.FunctionEndpoint}/api/sms"), message,
                new List<KeyValuePair<string, string>>
            {
               new KeyValuePair<string, string>("Authorization","HhMs8ZVg/HD4CzsN7ujGJsyWVmGmUDAVPv2a/t5c/vuiyh/zBrSTVg==")
            }, token).ConfigureAwait(false);
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("sms/verify"), ValidateModel]
        [Authorize(Policy = SignInStep.PolicyEmail)]
        public async Task<IActionResult> VerifySmsAsync([FromBody]CodeRequest model)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user).ConfigureAwait(false);
            var v = await _userManager.ChangePhoneNumberAsync(user, phoneNumber, model.Number).ConfigureAwait(false);
            if (v.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("userName")]
        [Authorize(Policy = SignInStep.PolicyPassword)]
        public IActionResult GetUserName()
        {
            var name = _userManager.GetUserName(User);
            return Ok(new { name });
        }

        [HttpPost("userName"), ValidateModel]
        [Authorize(Policy = SignInStep.PolicyPassword)]

        public async Task<IActionResult> ChangeUserNameAsync([FromBody]ChangeUserNameRequest model)
        {
            //TODO: check if this unique
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var result = await _userManager.SetUserNameAsync(user, model.Name).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("password")]
        [Authorize(Policy = SignInStep.PolicyPassword)]
        public async Task<IActionResult> GeneratePasswordAsync([FromServices] IBlockchainProvider blockchainProvider)
        {

            //TODO: check if i didn't generate a password
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var account = await blockchainProvider.CreateAccount();
            var privateKey = account.PrivateKey;
            var result = await _userManager.AddPasswordAsync(user, privateKey).ConfigureAwait(false);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                return Ok(
                new
                {
                    password = privateKey
                });
            }
            return BadRequest();
        }
    }
}