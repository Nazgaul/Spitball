using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;
using SbSignInManager = Cloudents.Web.Identity.SbSignInManager;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class LogInController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SbSignInManager _signInManager;
        private readonly ICommandBus _commandBus;
        private readonly IStringLocalizer<LogInController> _localizer;

        public LogInController(UserManager<User> userManager, SbSignInManager signInManager,
            IStringLocalizer<LogInController> localizer, ICommandBus commandBus)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
            _commandBus = commandBus;
        }

        // GET
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> PostAsync(
            [ModelBinder(typeof(CountryModelBinder))] string country,
            [FromBody]LoginRequest model,
            [FromHeader(Name = "User-Agent")] string? agent,
            CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                ModelState.AddModelError(nameof(model.Password), _localizer["BadLogin"]);
                return BadRequest(ModelState);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if (result == SignInResult.Success)
            {
                agent = agent?.Substring(0, Math.Min(agent.Length, 255));
                var command = new AddUserLocationCommand(user, country, HttpContext.GetIpAddress(), agent);
                var t1 = _commandBus.DispatchAsync(command, token);
                var t2 = _userManager.ResetAccessFailedCountAsync(user);
                var t3 = _signInManager.SignInAsync(user, false);
                await Task.WhenAll(t1, t2, t3);
                return Ok(new { user.Country });
            }


            if (result.IsLockedOut)
            {
                if (user.LockoutEnd == DateTimeOffset.MaxValue)
                {
                    ModelState.AddModelError(nameof(model.Password), _localizer["LockOut"]);
                    return BadRequest(ModelState);
                }

                ModelState.AddModelError(nameof(model.Password), _localizer["TempLockOut"]);
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


        [HttpGet("ValidateEmail")]
        [ResponseCache(Duration = TimeConst.Minute * 2, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new[] { nameof(EmailValidateRequest.Email) })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ReturnSignUserResponse>> CheckUserStatusAsync(
            [FromQuery] EmailValidateRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                ModelState.AddModelError(nameof(model.Email), _localizer["EmailNotFound"]);
                return BadRequest(ModelState);
            }

            if (!user.PhoneNumberConfirmed)
            {
                if (user.OldUser.GetValueOrDefault())
                {
                    return new ReturnSignUserResponse(RegistrationStep.RegisterSetEmailPassword);
                }
                ModelState.AddModelError(nameof(model.Email), _localizer["EmailNotFound"]);
                return BadRequest(ModelState);
            }

            if (user.PasswordHash is null)
            {
                return new ReturnSignUserResponse(RegistrationStep.RegisterSetEmailPassword);
            }
            return new ReturnSignUserResponse(RegistrationStep.LoginSetPassword);
        }
    }
}