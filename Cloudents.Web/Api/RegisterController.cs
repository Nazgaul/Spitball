using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IMailProvider _mailProvider;

        public RegisterController(
            SignInManager<User> signInManager,
            UserManager<User> userManager, IMailProvider mailProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mailProvider = mailProvider;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateUserAsync(RegisterEmailRequest model, CancellationToken token)
        {
            var user = new User
            {
                Email = model.Email,
                Name = model.Email
            };

            var p = await _userManager.CreateAsync(user).ConfigureAwait(false);
            if (p.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                var link = Url.Link("ConfirmEmail", new { user.Id, code });
                await _mailProvider.SendEmailAsync(model.Email, "Confirm your email",
                     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(link)}'>clicking here</a>.", token).ConfigureAwait(false);

                return Ok();
            }

            return BadRequest(p.Errors);
            //await _signInManager.SignInAsync(user, true).ConfigureAwait(false);
            //return Ok();
        }
    }
}