﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly SbSignInManager _signInManager;
        private readonly UserManager<User> _userManager;

        public LoginController(SbSignInManager signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        // GET

        [HttpPost]
        [ValidateModel, ValidateRecaptcha]
        public async Task<IActionResult> PostAsync(
            [FromBody] LoginRequest model,
            [FromServices] ISmsSender client,
            CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "email not found");
                return BadRequest(ModelState);
            }

            var taskSignIn = _signInManager.SignInTwoFactorAsync(user, false);

            var taskSms = client.SendSmsAsync(user, token);

            await Task.WhenAll(taskSms, taskSignIn).ConfigureAwait(false);
            return Ok();
            //if (result)
            //{
            //    return Ok();
            //}
            //ModelState.AddModelError(string.Empty, "email or password are invalid");
            //return BadRequest(ModelState);
        }



        [HttpPost("test")]
        [ValidateModel, ValidateRecaptcha]
        public async Task<IActionResult> PostAsync(

            CancellationToken token)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            
            //var t = await _userManager.GetUserAsync(User);
            return Ok();
            //var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            //if (user == null)
            //{
            //    ModelState.AddModelError(string.Empty, "email not found");
            //    return BadRequest(ModelState);
            //}


            //var result = await client.SendSmsAsync(user, token).ConfigureAwait(false);

            //if (result)
            //{
            //    return Ok();
            //}
            //ModelState.AddModelError(string.Empty, "email or password are invalid");
            //return BadRequest(ModelState);
        }
    }
}