﻿using System.Security.Claims;
using Cloudents.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Cloudents.Web.Identity
{
    public class SbSignInManager : SignInManager<RegularUser>
    {
        //public async Task<SignInResult> SignInTwoFactorAsync(User user, bool isPersistent)
        //{
        //    var signInResult = await SignInOrTwoFactorAsync(user, isPersistent);
        //    if (signInResult.Succeeded)
        //    {
        //        await SignOutAsync();
        //        return SignInResult.Failed;
        //    }
        //    return signInResult;
        //}


        public async Task TempSignIn(RegularUser user)
        {

            var userId = await UserManager.GetUserIdAsync(user);
            await Context.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, StoreTwoFactorInfo(userId, null));
        }


        private ClaimsPrincipal StoreTwoFactorInfo(string userId, string loginProvider)
        {
            var identity = new ClaimsIdentity(IdentityConstants.TwoFactorUserIdScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, userId));
            if (loginProvider != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, loginProvider));
            }
            return new ClaimsPrincipal(identity);
        }
        //public async Task TempSignIn(User user)
        //{
        //   this.GetTwoFactorAuthenticationUserAsync()
        //    var userId = await UserManager.GetUserIdAsync(user);
        //    await Context.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, StoreTwoFactorInfo(userId, loginProvider));
        //}

        //protected override Task<SignInResult> SignInOrTwoFactorAsync(User user, bool isPersistent, string loginProvider = null, bool bypassTwoFactor = true)
        //{
        //    return base.SignInOrTwoFactorAsync(user, isPersistent, loginProvider, bypassTwoFactor);
        //}

        public SbSignInManager(UserManager<RegularUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<RegularUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<RegularUser>> logger, IAuthenticationSchemeProvider schemes) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }


        /*public virtual async Task<SignInResult> PasswordSignInAsync(TUser user, string password,
            bool isPersistent, bool lockoutOnFailure)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var attempt = await CheckPasswordSignInAsync(user, password, lockoutOnFailure);
            return attempt.Succeeded
                ? await SignInOrTwoFactorAsync(user, isPersistent)
                : attempt;
        }*/
    }
}
