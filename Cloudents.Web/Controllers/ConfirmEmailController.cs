using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Api;
using Cloudents.Web.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]", Name = "ConfirmEmail")]
    public class ConfirmEmailController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SbSignInManager _signInManager;

        public ConfirmEmailController(UserManager<User> userManager, SbSignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET
        public async Task<IActionResult> Index(string id, string code, CancellationToken token)
        {
            if (id == null || code == null)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            TempData.Remove(RegisterController.Email);
            var user = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{id}'.");
            }

            //var taskBlockChain = _serviceBusProvider.InsertMessageAsync(
            //    new BlockChainInitialBalance(_blockChain.GetAddress(user.PrivateKey)), token);
            var result = await _userManager.ConfirmEmailAsync(user, code).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Error confirming email for user with ID '{id}':");
            }

            await _signInManager.SignInTwoFactorAsync(user, false).ConfigureAwait(false);

            //await taskBlockChain.ConfigureAwait(false);
            return Redirect("/verify-phone");
        }
    }
}