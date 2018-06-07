using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
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
        private readonly IQueueProvider _queueProvider;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public RegisterController(
            UserManager<User> userManager, IConfigurationKeys configuration, IQueueProvider queueProvider, SignInManager<User> signInManager)
        {
            _userManager = userManager;
           
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
            if (User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError(string.Empty, "user is already logged in");
                return BadRequest(ModelState);
            }
            var account = Infrastructure.BlockChain.BlockChainProvider.CreateAccount();
            
            var userName = model.Email.Split(new[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
            var user = new User
            {
                Email = model.Email,
                Name = userName + GenerateRandomNumber(),
                TwoFactorEnabled = true,
                PrivateKey = account.privateKey
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
            ModelState.AddIdentityModelError(p);
            return BadRequest(ModelState);
        }

        //TODO: authorize is no good.
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
                ModelState.AddModelError(string.Empty, "No result from google");
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Email = result.Email,
                Name = result.Name + GenerateRandomNumber(),
                EmailConfirmed = true,
                TwoFactorEnabled = true
            };
            var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
            if (p.Succeeded)
            {
               // await _signInManager.SignInAsync(user, false).ConfigureAwait(false);
                return Ok();
            }
            ModelState.AddIdentityModelError(p);
            return BadRequest(ModelState);
        }

    }
}