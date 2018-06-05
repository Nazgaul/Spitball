using System;
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

        private static int GenerateRandomNumber()
        {
            var rdm = new Random();
            return rdm.Next(1000, 9999);
        }

        [HttpPost]
        [ValidateModel, ValidateRecaptcha]
        public async Task<IActionResult> CreateUserAsync([FromBody]RegisterEmailRequest model, CancellationToken token)
        {
            var userName = model.Email.Split(new[] { '.', '@' }, 1, StringSplitOptions.RemoveEmptyEntries)[0];
            var user = new User
            {
                Email = model.Email,
                Name = userName + GenerateRandomNumber()
            };

            var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
            if (p.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                var link = Url.Link("ConfirmEmail", new { user.Id, code });

                var message = new RegistrationEmail(model.Email, HtmlEncoder.Default.Encode(link));
                await _queueProvider.InsertEmailMessageAsync(message, token).ConfigureAwait(false);
                return Ok();
            }
            return BadRequest(p.Errors);
        }

        [HttpPost("resend"), Authorize]
        public async Task<IActionResult> ResendEmailAsync(CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
            var link = Url.Link("ConfirmEmail", new { user.Id, code });
            var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link));
            await _queueProvider.InsertEmailMessageAsync(message, token).ConfigureAwait(false);
            return Ok();
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
                Name = result.Name + GenerateRandomNumber(),
                EmailConfirmed = true
            };
            var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
            if (p.Succeeded)
            {
                await _signInManager.SignInAsync(user, false).ConfigureAwait(false);
                return Ok();
            }
            return BadRequest(p.Errors);
        }

        [HttpPost("sms"), ValidateModel]
        [Authorize]
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

            var result = await client.PostJsonAsync(new Uri($"{_configuration.FunctionEndpoint}/api/sms?code=HhMs8ZVg/HD4CzsN7ujGJsyWVmGmUDAVPv2a/t5c/vuiyh/zBrSTVg=="), message,
            null, token).ConfigureAwait(false);
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("sms/verify"), ValidateModel]
        [Authorize]
        public async Task<IActionResult> VerifySmsAsync([FromBody]CodeRequest model)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user).ConfigureAwait(false);
            var v = await _userManager.ChangePhoneNumberAsync(user, phoneNumber, model.Number).ConfigureAwait(false);
            if (v.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("password")]
        [Authorize]
        public async Task<IActionResult> GeneratePasswordAsync(
            [FromServices] IBlockChainErc20Service blockChainErc20Service,
            [FromServices] IQueueProvider client,
            CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var account = Infrastructure.BlockChain.BlockChainProvider.CreateAccount();

            var t1 = blockChainErc20Service.SetInitialBalanceAsync(account.publicAddress, token);

            var t3 = client.InsertBackgroundMessageAsync(new TalkJsUser(user.Id)
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.PhoneNumberHash
            }, token);



            var privateKey = account.privateKey;
            var t2 = _userManager.AddPasswordAsync(user, privateKey);

            await Task.WhenAll(t1, t2, t3).ConfigureAwait(false);
            if (t2.Result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false).ConfigureAwait(false);
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