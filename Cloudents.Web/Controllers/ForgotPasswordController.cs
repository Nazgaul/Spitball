using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ForgotPasswordController : Controller
    {
        private readonly UserManager<RegularUser> _userManager;

        public ForgotPasswordController(UserManager<RegularUser> userManager)
        {
            _userManager = userManager;
        }

        // GET
        [Route("resetPassword", Name = "ResetPassword")]
        public async Task<IActionResult> Index(long id, string code)
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
            var isValidToken = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, UserManager<RegularUser>.ResetPasswordTokenPurpose, code);
            if (!isValidToken)
            {
                return RedirectToRoute(RegisterController.Signin);
            }
            return View();
        }
    }
}