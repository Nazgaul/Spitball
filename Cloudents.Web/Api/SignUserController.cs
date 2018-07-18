using System;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SignUserController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SbSignInManager _signInManager;
        private readonly ISmsSender _smsClient;
        private readonly IBlockChainErc20Service _blockChainErc20Service;
        private readonly IServiceBusProvider _queueProvider;
        internal const string Email = "email";


        private enum NextStep
        {
            EmailConfirmed,
            VerifyPhone,
            EnterPhone
        }

        //private class SignUserModel
        //{
        //    public string Email { get; set; }

        //}

        public SignUserController(UserManager<User> userManager, SbSignInManager signInManager, ISmsSender smsClient, IBlockChainErc20Service blockChainErc20Service, IServiceBusProvider queueProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _smsClient = smsClient;
            _blockChainErc20Service = blockChainErc20Service;
            _queueProvider = queueProvider;
        }


        


        [HttpPost, ValidateModel, ValidateRecaptcha]
        public async Task<IActionResult> SignUser([FromBody]SignUserRequest model, CancellationToken token)
        {
            if (User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError(string.Empty, "user is already logged in");
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
            {
                user = CreateUser(model.Email,model.Name);
                user.EmailConfirmed = model.EmailConfirmed;

                var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
                if (p.Succeeded)
                {
                    if (!user.EmailConfirmed)
                    {
                        await GenerateEmailAsync(user, token).ConfigureAwait(false);
                        return Ok(new
                        {
                            step = NextStep.EmailConfirmed
                        });
                    }
                    return Ok(new
                    {
                        step = NextStep.EnterPhone
                    });
                }
                ModelState.AddIdentityModelError(p);
                return BadRequest(ModelState);
            }
            if (!user.EmailConfirmed || !user.PhoneNumberConfirmed)
            {
                await GenerateEmailAsync(user, token).ConfigureAwait(false);
                return Ok(new
                {
                    step = NextStep.EmailConfirmed
                });
            }
            var taskSignIn = _signInManager.SignInTwoFactorAsync(user, false);
            var taskSms = _smsClient.SendSmsAsync(user, token);

            TempData["SMS"] = user.Email;
            await Task.WhenAll(taskSms, taskSignIn).ConfigureAwait(false);

            if (taskSignIn.Result.RequiresTwoFactor)
            {
                return Ok(new
                {
                    step = NextStep.VerifyPhone
                });
            }
            ModelState.AddModelError(string.Empty, taskSignIn.Result.ToString());
            return BadRequest(ModelState);


        }


        private async Task GenerateEmailAsync(User user, CancellationToken token)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
            var link = Url.Link("ConfirmEmail", new { user.Id, code });
            TempData[Email] = user.Email;
            var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link));
            await _queueProvider.InsertMessageAsync(message, token).ConfigureAwait(false);
        }


        [HttpPost("google"), ValidateModel]
        public async Task<IActionResult> GoogleSignInAsync([FromBody] TokenRequest model,
            [FromServices] IGoogleAuth service,
            [FromServices] SbSignInManager signInManager,
            CancellationToken cancellationToken)
        {
            var result = await service.LogInAsync(model.Token, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "No result from google");
                return BadRequest(ModelState);
            }

            return await SignUser(new SignUserRequest(result.Email, true, result.Name), cancellationToken);

            //var user = CreateUser(result.Email, result.Name);
            //user.EmailConfirmed = true;

            //var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
            //if (p.Succeeded)
            //{
            //    //TODO: duplicate link confirm email.
            //    var t2 = signInManager.SignInTwoFactorAsync(user, false);
            //    await Task.WhenAll(/*t1,*/ t2).ConfigureAwait(false);
            //    return Ok();
            //}
            //ModelState.AddIdentityModelError(p);
            //return BadRequest(ModelState);
        }



        //private User CreateUser(string email)
        //{
        //    var userName = email.Split(new[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
        //    return CreateUser(email, userName);
        //}

        private User CreateUser([NotNull] string email, string name)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(name))
            {
                name = email.Split(new[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            var (privateKey, _) = _blockChainErc20Service.CreateAccount();
            return new User(email, $"{name}.{GenerateRandomNumber()}", privateKey);
        }

        private static int GenerateRandomNumber()
        {
            var rdm = new Random();
            return rdm.Next(1000, 9999);
        }


        [HttpPost("resend")]
        public async Task<IActionResult> ResendEmailAsync(
            CancellationToken token)
        {
            var email = TempData.Peek(Email) ?? throw new ArgumentNullException("TempData", "email is empty");
            var user = await _userManager.FindByEmailAsync(email.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "no user");
                return BadRequest(ModelState);
            }

            await GenerateEmailAsync(user, token).ConfigureAwait(false);
            return Ok();
        }

    }
}