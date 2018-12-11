using Cloudents.Core.Entities.Db;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class LogInController : ControllerBase
    {
        private readonly UserManager<RegularUser> _userManager;
        private readonly SbSignInManager _signInManager;
        private readonly ICommandBus _commandBus;
        private readonly IStringLocalizer<LogInController> _localizer;

        public LogInController(UserManager<RegularUser> userManager, SbSignInManager signInManager,
            IStringLocalizer<LogInController> localizer, ICommandBus commandBus)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
            _commandBus = commandBus;
        }

        // GET
        [HttpPost]
        public async Task<ActionResult> Post(
            [ModelBinder(typeof(CountryModelBinder))] string country,
            [FromBody]LoginRequest model,
            CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Password), _localizer["BadLogin"]);
                return BadRequest(ModelState);

            }

            var command = new AddUserLocationCommand(user, country, HttpContext.Connection.GetIpAddress());
            var t1 = _commandBus.DispatchAsync(command, token);
            var t2 = _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            await Task.WhenAll(t1, t2);
            var result = t2.Result;
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
            ModelState.AddModelError(nameof(model.Password), _localizer["BadLogin"]);
            return BadRequest(ModelState);


        }


        [HttpPost("ValidateEmail")]
        public async Task<ActionResult<CheckUserStatusResponse>> CheckUserStatus([FromBody] EmailValidateRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Email), _localizer["EmailNotFound"]);
                return BadRequest(ModelState);
            }

            if (user.PasswordHash == null)
            {
                return new CheckUserStatusResponse(NextStep.EmailPassword);
            }

            return new CheckUserStatusResponse(NextStep.Loginstep);
        }
    }
}