using System;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [Route("[controller]", Name = "ConfirmEmail")]
    public class ConfirmEmailController : Controller
    {
        private readonly UserManager<User> _userManager;

        public ConfirmEmailController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET
        public async Task<IActionResult> Index(string id, string code)
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

            var result = await _userManager.ConfirmEmailAsync(user, code).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Error confirming email for user with ID '{id}':");
            }

            return View();
        }
    }
}