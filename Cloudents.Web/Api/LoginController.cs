﻿using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public LoginController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        // GET

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post(LoginRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
            {
                return BadRequest();
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Key, false, false).ConfigureAwait(false);

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}