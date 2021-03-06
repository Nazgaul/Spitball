﻿using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Microsoft.ApplicationInsights;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class LogInController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ICommandBus _commandBus;
        private readonly TelemetryClient _logClient;
        private readonly IStringLocalizer<LogInController> _localizer;

        public LogInController(UserManager<User> userManager, SignInManager<User> signInManager,
            IStringLocalizer<LogInController> localizer, ICommandBus commandBus, TelemetryClient logClient)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
            _commandBus = commandBus;
            _logClient = logClient;
        }

        // GET
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> PostAsync(
            [FromBody]LoginRequest model,
            [FromHeader(Name = "User-Agent")] string? agent,
            CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                _logClient.TrackTrace("can find user", model.AsDictionary());
                ModelState.AddModelError(nameof(model.Password), _localizer["EmailNotFound"]);
                return BadRequest(ModelState);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if (result == SignInResult.Success)
            {
                agent = agent?.Substring(0, Math.Min(agent.Length, 255));
                var command = new AddUserLocationCommand(user,  HttpContext.GetIpAddress(), agent);
                var t1 = _commandBus.DispatchAsync(command, token);
                var t2 = _userManager.ResetAccessFailedCountAsync(user);
                var t3 = _signInManager.SignInAsync(user, true);
                await Task.WhenAll(t1, t2, t3);
                return Ok(new { user.Country });
            }

            if (result.IsLockedOut)
            {
                if (user.LockoutEnd == DateTimeOffset.MaxValue)
                {
                    _logClient.TrackTrace("user locked out", model.AsDictionary());
                    ModelState.AddModelError(nameof(model.Password), _localizer["LockOut"]);
                    return BadRequest(ModelState);
                }

                _logClient.TrackTrace("user temp locked out", model.AsDictionary());
                ModelState.AddModelError(nameof(model.Password), _localizer["TempLockOut"]);
                return BadRequest(ModelState);
            }


            if (result.IsNotAllowed)
            {
                _logClient.TrackTrace("user is not allowed", model.AsDictionary());
                ModelState.AddModelError(nameof(model.Password), _localizer["NotAllowed"]);
                return BadRequest(ModelState);
            }
            ModelState.AddModelError(nameof(model.Password), _localizer["BadLogin"]);
            return BadRequest(ModelState);
        }
    }
}