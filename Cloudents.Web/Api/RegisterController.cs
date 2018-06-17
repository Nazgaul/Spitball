using System;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {
        private readonly IServiceBusProvider _queueProvider;
        private readonly UserManager<User> _userManager;
        private readonly IBlockChainErc20Service _blockChainErc20Service;

        public RegisterController(
            UserManager<User> userManager, IServiceBusProvider queueProvider, IBlockChainErc20Service blockChainErc20Service)
        {
            _userManager = userManager;
            _queueProvider = queueProvider;
            _blockChainErc20Service = blockChainErc20Service;
        }

        private static int GenerateRandomNumber()
        {
            var rdm = new Random();
            return rdm.Next(1000, 9999);
        }

        [HttpPost]
        [ValidateModel, ValidateRecaptcha]
        public async Task<IActionResult> CreateUserAsync([FromBody]RegisterEmailRequest model,
            CancellationToken token)
        {
            if (User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError(string.Empty, "user is already logged in");
                return BadRequest(ModelState);
            }
            var (privateKey, _) = _blockChainErc20Service.CreateAccount();
            var userName = model.Email.Split(new[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
            var user = new User(model.Email, $"{userName}.{GenerateRandomNumber()}", privateKey);

            var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
            if (p.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                var link = Url.Link("ConfirmEmail", new { user.Id, code });
                TempData["email"] = model.Email;
                var message = new RegistrationEmail(model.Email, HtmlEncoder.Default.Encode(link));
                await _queueProvider.InsertMessageAsync(message, token).ConfigureAwait(false);
                return Ok();
            }
            ModelState.AddIdentityModelError(p);
            return BadRequest(ModelState);
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendEmailAsync(
            CancellationToken token)
        {
            var email = TempData["email"];
            var user = await _userManager.FindByEmailAsync(email.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "no user");
                return BadRequest(ModelState);
            }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
            var link = Url.Link("ConfirmEmail", new { user.Id, code });
            var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link));
            await _queueProvider.InsertMessageAsync(message, token).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("google"), ValidateModel]
        public async Task<IActionResult> GoogleSignInAsync([FromBody] TokenRequest model,
            [FromServices] IGoogleAuth service,
            [FromServices] IServiceBusProvider serviceBusProvider,
            [FromServices] SbSignInManager signInManager,
            CancellationToken cancellationToken)
        {
            var result = await service.LogInAsync(model.Token, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "No result from google");
                return BadRequest(ModelState);
            }

            var account = _blockChainErc20Service.CreateAccount();

            var user = new User(result.Email, $"{result.Name}.{GenerateRandomNumber()}", account.privateKey)
            {
                EmailConfirmed = true
            };

            var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
            if (p.Succeeded)
            {
                //TODO: duplicate link confirm email.
                var t1 = serviceBusProvider.InsertMessageAsync(
                    new BlockChainInitialBalance(account.publicAddress), cancellationToken);
                var t2 = signInManager.SignInTwoFactorAsync(user, false);
                await Task.WhenAll(t1, t2).ConfigureAwait(false);
                return Ok();
            }
            ModelState.AddIdentityModelError(p);
            return BadRequest(ModelState);
        }
    }
}