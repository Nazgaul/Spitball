using Cloudents.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ForgotPasswordController : Controller
    {
        internal const string ResetPasswordRouteName = "ResetPassword";
        private readonly UserManager<User> _userManager;

        public ForgotPasswordController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET
        [Route("signin/reset-password", Name = ResetPasswordRouteName)]
        public async Task<IActionResult> IndexAsync(long id, string code)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return Redirect("/");
            }
            code = System.Net.WebUtility.UrlDecode(code);
            var isValidToken = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, UserManager<User>.ResetPasswordTokenPurpose, code);
            if (!isValidToken)
            {
                return RedirectToRoute(RegisterController.Signin);
            }
            return View("Index");
        }
    }
}