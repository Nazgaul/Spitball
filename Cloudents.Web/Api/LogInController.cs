using Cloudents.Core.Entities.Db;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class LogInController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LogInController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]LoginRequest model, CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("email of password are incorrect");
                return BadRequest(ModelState);

            }
            //var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if (result == SignInResult.Success)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok();
            }

            //if (result == SignInResult.TwoFactorRequired)
            //{
            //    var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
            //    await _smsClient.SendSmsAsync(user.PhoneNumber, code, token);
            //    return Ok(new ReturnSignUserResponse(NextStep.VerifyPhone, false));
            //}

            if (result.IsLockedOut)
            {
                //TODO: Localize
                ModelState.AddModelError(string.Empty, "user is locked out");
                return BadRequest(ModelState);
            }

            if (result.IsNotAllowed)
            {
                //TODO: Localize
                ModelState.AddModelError(string.Empty, "user is not allowed");
                return BadRequest(ModelState);

            }
            //TODO: Localize
            ModelState.AddModelError("email of password are incorrect");
            return BadRequest(ModelState);

           
        }
    }
}