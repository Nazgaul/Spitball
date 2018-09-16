using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"),ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IServiceBusProvider _queueProvider;


        public ForgotPasswordController(UserManager<User> userManager, SignInManager<User> signInManager, IServiceBusProvider queueProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _queueProvider = queueProvider;
        }

        // GET
        [HttpPost]
        public async Task<IActionResult> Post(ForgotPasswordRequest model,[FromHeader] CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return BadRequest();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = UrlEncoder.Default.Encode(code);
            var link = Url.Link("ResetPassword", new { user.Id, code });
            var message = new ResetPasswordEmail(user.Email, link);
            await _queueProvider.InsertMessageAsync(message, token).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model, CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest();
            }
            model.Code = System.Net.WebUtility.UrlDecode(model.Code);
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                var result2 = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);

                return Ok();
            }
            ModelState.AddIdentityModelError(result);
            return BadRequest(ModelState);
        }
    }
}