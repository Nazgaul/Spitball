using Cloudents.Admin2.Models;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Controllers
{
    [Route("[controller]/[action]")]

    public class AccountController : Controller
    {
        private readonly IQueryBus _queryBus;

        public AccountController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LogIn()
        {
            return View();
            //var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
            //return Challenge(
            //    new AuthenticationProperties { RedirectUri = redirectUrl },
            //    OpenIdConnectDefaults.AuthenticationScheme);
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogInGoogle([FromBody] GoogleLogInRequest model,
            [FromServices] IGoogleAuth service, CancellationToken cancellationToken)
        {
            var login = await service.LogInAsync(model.Token, cancellationToken);
            if (login is null)
            {
                return BadRequest();
            }
            var query = new ValidateUserQuery(login.Email);
            var result = await _queryBus.QueryAsync(query, cancellationToken);
            if (result is null)
            {
                return Unauthorized();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login.Email),
                new Claim("FullName", $"{login.FirstName} { login.LastName}"),
                new Claim("UserId", result.Id.ToString()),
                new Claim(ClaimsPrincipalExtensions.ClaimCountry, result.Country ?? "None"),
            };
            //foreach (var resultRole in result.Roles ?? Enumerable.Empty<string>())
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, resultRole));
            //}

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = false,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            return Ok();
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
