using System;
using System.Linq;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    //[Produces("application/json")]
    //[Route("api/[controller]")]
    //public class RegisterController : Controller
    //{
    //    internal const string Email = "email";
    //    private readonly IServiceBusProvider _queueProvider;
    //    private readonly UserManager<User> _userManager;
    //    private readonly IBlockChainErc20Service _blockChainErc20Service;

    //    public RegisterController(
    //        UserManager<User> userManager, IServiceBusProvider queueProvider, IBlockChainErc20Service blockChainErc20Service)
    //    {
    //        _userManager = userManager;
    //        _queueProvider = queueProvider;
    //        _blockChainErc20Service = blockChainErc20Service;
    //    }

        //private static int GenerateRandomNumber()
        //{
        //    var rdm = new Random();
        //    return rdm.Next(1000, 9999);
        //}



        






        //[HttpPost]
        //[ValidateModel, ValidateRecaptcha]
        //public async Task<IActionResult> CreateUserAsync([FromBody]RegisterEmailRequest model,
        //    CancellationToken token)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        ModelState.AddModelError(string.Empty, "user is already logged in");
        //        return BadRequest(ModelState);
        //    }

        //    var user = CreateUser(model.Email);

        //    var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
        //    if (p.Succeeded)
        //    {
        //        await GenerateEmailAsync(user, token).ConfigureAwait(false);
        //        return Ok();
        //    }

        //    if (p.Errors.Any(f => string.Equals(f.Code, "duplicateEmail", StringComparison.OrdinalIgnoreCase)))
        //    {
        //        user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        //        if (!user.EmailConfirmed || !user.PhoneNumberConfirmed)
        //        {
        //            await GenerateEmailAsync(user, token).ConfigureAwait(false);
        //            return Ok();
        //        }
        //    }

        //    ModelState.AddIdentityModelError(p);
        //    return BadRequest(ModelState);
        //}

        //private async Task GenerateEmailAsync(User user, CancellationToken token)
        //{
        //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
        //    var link = Url.Link("ConfirmEmail", new { user.Id, code });
        //    TempData[Email] = user.Email;
        //    var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link));
        //    await _queueProvider.InsertMessageAsync(message, token).ConfigureAwait(false);
        //}

        //private User CreateUser(string email)
        //{
        //    var userName = email.Split(new[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
        //    return CreateUser(email, userName);
        //}

        //private User CreateUser(string email, string name)
        //{
        //    var (privateKey, _) = _blockChainErc20Service.CreateAccount();
        //    return new User(email, $"{name}.{GenerateRandomNumber()}", privateKey);
        //}

        //[HttpPost("google"), ValidateModel]
        //public async Task<IActionResult> GoogleSignInAsync([FromBody] TokenRequest model,
        //    [FromServices] IGoogleAuth service,
        //    [FromServices] SbSignInManager signInManager,
        //    CancellationToken cancellationToken)
        //{
        //    var result = await service.LogInAsync(model.Token, cancellationToken).ConfigureAwait(false);
        //    if (result == null)
        //    {
        //        ModelState.AddModelError(string.Empty, "No result from google");
        //        return BadRequest(ModelState);
        //    }
        //    var user = CreateUser(result.Email, result.Name);
        //    user.EmailConfirmed = true;

        //    var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
        //    if (p.Succeeded)
        //    {
        //        //TODO: duplicate link confirm email.
        //        var t2 = signInManager.SignInTwoFactorAsync(user, false);
        //        await Task.WhenAll(/*t1,*/ t2).ConfigureAwait(false);
        //        return Ok();
        //    }
        //    ModelState.AddIdentityModelError(p);
        //    return BadRequest(ModelState);
        //}

        //[HttpPost("resend")]
        //public async Task<IActionResult> ResendEmailAsync(
        //    CancellationToken token)
        //{
        //    var email = TempData.Peek(Email) ?? throw new ArgumentNullException("TempData", "email is empty");
        //    var user = await _userManager.FindByEmailAsync(email.ToString()).ConfigureAwait(false);
        //    if (user == null)
        //    {
        //        ModelState.AddModelError(string.Empty, "no user");
        //        return BadRequest(ModelState);
        //    }

        //    await GenerateEmailAsync(user, token).ConfigureAwait(false);
        //    return Ok();
        //}
   // }
}