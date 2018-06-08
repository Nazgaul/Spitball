using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
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
        private readonly IBlockChainErc20Service _blockChain;

        public ConfirmEmailController(UserManager<User> userManager, SbSignInManager signInManager, IBlockChainErc20Service blockChain)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _blockChain = blockChain;
        }

        // GET
        public async Task<IActionResult> Index(string id, string code, CancellationToken token)
        {
            if (id == null || code == null)
            {
                return RedirectToAction(nameof(Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{id}'.");
            }
            var taskBlockChain = _blockChain.SetInitialBalanceAsync(_blockChain.GetAddress(user.PrivateKey), token);
            var result = await _userManager.ConfirmEmailAsync(user, code).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Error confirming email for user with ID '{id}':");
            }

            await _signInManager.SignInTwoFactorAsync(user, false).ConfigureAwait(false);

            await taskBlockChain.ConfigureAwait(false);
            return Redirect("/verify-phone");
        }
    }
}