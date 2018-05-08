using System;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Filters;
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
        private readonly IMailProvider _mailProvider;

        public RegisterController(
            UserManager<User> userManager, IMailProvider mailProvider)
        {
            _userManager = userManager;
            _mailProvider = mailProvider;
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
                await _mailProvider.SendEmailAsync(model.Email, "Confirm your email",
                     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(link)}'>clicking here</a>.", token).ConfigureAwait(false);
                return Ok();
            }

            //await _signInManager.SignInAsync(user, false);
            return BadRequest(p.Errors);
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleSigninAsync([NotNull] string token, [FromServices] IGoogleAuth service, CancellationToken cancellationToken)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            var result = await service.LogInAsync(token, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return BadRequest();

            }

            return Ok();
            //var user = new User
            //{
            //    Email = result,
            //    Name = model.Email
            //};

            //var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
        }

        [HttpPost("sms")]
        [Authorize]
        public async Task<IActionResult> SmsUserAsync(string phoneNumber, [FromServices] ISmsProvider smsProvider)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            await _userManager.SetPhoneNumberAsync(user, phoneNumber).ConfigureAwait(false);
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber).ConfigureAwait(false);
            await smsProvider.SendSmsAsync(phoneNumber, code).ConfigureAwait(false);
            return Ok();
        }


        [HttpPost("sms/verify")]
        [Authorize]
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
        [Authorize]
        public IActionResult GetUserName()
        {
            var name = _userManager.GetUserName(User);
            return Ok(new { name });
        }

        [HttpPost("userName")]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> GeneratePasswordAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            //TODO: generate private key in here
            var privateKey = "maybe some phrase to put in here";
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