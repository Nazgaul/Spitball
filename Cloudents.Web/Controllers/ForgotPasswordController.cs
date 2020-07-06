using Cloudents.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ForgotPasswordController : Controller
    {
        internal const string ResetPasswordRouteName = "ResetPassword";
        private readonly UserManager<User> _userManager;
        private readonly TelemetryClient _telemetryClient;

        public ForgotPasswordController(UserManager<User> userManager, TelemetryClient telemetryClient)
        {
            _userManager = userManager;
            _telemetryClient = telemetryClient;
        }

        // GET
        [Route("signin/reset-password", Name = ResetPasswordRouteName)]
        public async Task<IActionResult> IndexAsync(long id, string code)
        {
            if (User.Identity.IsAuthenticated)
            {
                _telemetryClient.TrackTrace($"User is registered");
                return Redirect("/");
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _telemetryClient.TrackTrace($"Cant find user {id}");
                return Redirect("/");
            }
            code = System.Net.WebUtility.UrlDecode(code);
            var isValidToken = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, UserManager<User>.ResetPasswordTokenPurpose, code);
            if (!isValidToken)
            {
                _telemetryClient.TrackTrace($"Not valid token");
                return Redirect("/");
                //return RedirectToRoute(RegisterController.Signin);
            }
            return View("Index");
        }
    }
}