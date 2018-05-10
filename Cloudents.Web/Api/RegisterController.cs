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
using JetBrains.Annotations;
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
        public async Task<IActionResult> CreateUserAsync(RegisterEmailRequest model, CancellationToken token)
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
                    PlaceHolders = new[] { HtmlEncoder.Default.Encode(link) },
                    Template = "register",
                    Subject = "welcome to spitball"
                };
                await _queueProvider.InsertMessageAsync(message, token);
                await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                //await _mailProvider.SendEmailAsync(model.Email, "Confirm your email",
                //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(link)}'>clicking here</a>.", token).ConfigureAwait(false);
                return Ok();
            }

            //await _signInManager.SignInAsync(user, false);
            return BadRequest(p.Errors);
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleSigninAsync([NotNull] string token,
            [FromServices] IGoogleAuth service,
            CancellationToken cancellationToken)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            var result = await service.LogInAsync(token, cancellationToken).ConfigureAwait(false);
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

        [HttpPost("sms")]
        [Authorize(Policy = SignInStep.PolicyEmail)]
        public async Task<IActionResult> SmsUserAsync(string phoneNumber, [FromServices] IRestClient client, CancellationToken token)
        {
            
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            await _userManager.SetPhoneNumberAsync(user, phoneNumber).ConfigureAwait(false);
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber).ConfigureAwait(false);

            var message = new SmsMessage
            {
                PhoneNumber = phoneNumber,
                Message = code
            };
            //TODO: change url
            var result = await client.PostJsonAsync(new Uri($"{_configuration.FunctionEndpoint}/api/sms"), message,
                new List<KeyValuePair<string, string>>
            {
               new KeyValuePair<string, string>("Authorization","HhMs8ZVg/HD4CzsN7ujGJsyWVmGmUDAVPv2a/t5c/vuiyh/zBrSTVg==")
            }, token);
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("sms/verify")]
        [Authorize(Policy = SignInStep.PolicyEmail)]
        public async Task<IActionResult> VerifySmsAsync(string code)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var v = await _userManager.ChangePhoneNumberAsync(user, phoneNumber, code).ConfigureAwait(false);
            if (v.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("userName")]
        [Authorize(Policy = SignInStep.PolicySms)]
        public IActionResult GetUserName()
        {
            var name = _userManager.GetUserName(User);
            return Ok(new { name });
        }

        [HttpPost("userName")]
        [Authorize(Policy = SignInStep.PolicySms)]

        public async Task<IActionResult> ChangeUserNameAsync(string userName)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var result = await _userManager.SetUserNameAsync(user, userName);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("password")]
        [Authorize(Policy = SignInStep.PolicySms)]
        public async Task<IActionResult> GeneratePasswordAsync([FromServices] IBlockchainProvider blockchainProvider)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var privateKey = blockchainProvider.CreateAccount();
            var result = await _userManager.AddPasswordAsync(user, privateKey);
            if (result.Succeeded)
            {
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