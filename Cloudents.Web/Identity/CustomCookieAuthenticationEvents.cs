using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Identity
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        internal const string ValidateTimeClaim = "ValidateTime";
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        public CustomCookieAuthenticationEvents(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var claim = context.Principal.FindFirst(ValidateTimeClaim);
            var lastCheck = DateTime.UtcNow.AddDays(-1);
            if (claim?.Value != null)
            {
                DateTime.TryParse(claim.Value, out lastCheck);
            }

            if (lastCheck < DateTime.UtcNow.AddMinutes(-5))
            {
                var user = await _userManager.GetUserAsync(context.Principal);
                if (user == null)
                {
                    context.RejectPrincipal();
                    await _signInManager.SignOutAsync();
                    return;
                }
                if (user.LockoutEnd.HasValue && DateTime.UtcNow < user.LockoutEnd.Value)
                {
                    context.RejectPrincipal();
                    await _signInManager.SignOutAsync();
                }
                else
                {
                    context.ShouldRenew = true;
                    var identity = (ClaimsIdentity)context.Principal.Identity;
                    identity.TryRemoveClaim(claim);
                    identity.AddClaim(new Claim(ValidateTimeClaim, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));
                    //var claims = context.Principal.Claims.ToList();
                    //claims.Add(new Claim("ValidateTime", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));
                    //var claimsIdentity = new ClaimsIdentity(
                    //    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    context.ReplacePrincipal(new ClaimsPrincipal(identity));
                }
            }

           

            //var userPrincipal = context.Principal;

            //// Look for the LastChanged claim.
            //var lastChanged = (from c in userPrincipal.Claims
            //    where c.Type == "LastChanged"
            //    select c.Value).FirstOrDefault();

            //if (string.IsNullOrEmpty(lastChanged) ||
            //    !_userRepository.ValidateLastChanged(lastChanged))
            //{
            //    context.RejectPrincipal();

            //    await context.HttpContext.SignOutAsync(
            //        CookieAuthenticationDefaults.AuthenticationScheme);
            //}
        }
        /*c.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                    c.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };*/
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    }
}