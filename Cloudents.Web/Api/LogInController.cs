using Cloudents.Core.Entities.Db;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class LogInController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IStringLocalizer<LogInController> _localizer;

        public LogInController(UserManager<User> userManager, SignInManager<User> signInManager, 
            IStringLocalizer<LogInController> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
        }

        // GET
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]LoginRequest model, CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Password), _localizer["BadLogin"]);
                return BadRequest(ModelState);

            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if (result == SignInResult.Success)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok();
            }

            
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(nameof(model.Password), _localizer["LockOut"]);
                return BadRequest(ModelState);
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(nameof(model.Password), _localizer["NotAllowed"]);
                return BadRequest(ModelState);

            }
            ModelState.AddModelError(nameof(model.Password),_localizer["BadLogin"]);
            return BadRequest(ModelState);

           
        }


        [HttpPost("ValidateEmail")]
        public async Task<ActionResult<NextStep>> CheckUserStatus([FromBody] EmailValidateRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Email), _localizer["BadLogin"]);
                return BadRequest(ModelState);

            }

            if (user.PasswordHash == null)
            {
                return NextStep.EmailPassword;
            }

            return NextStep.Loginstep;
        }
    }
}