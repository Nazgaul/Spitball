using Cloudents.Core.Entities.Db;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Cloudents.Web.Identity
{
    //[UsedImplicitly]
    //public class PasswordHasher : IPasswordHasher<User>
    //{
    //    public string HashPassword(User user, string password)
    //    {
    //        return Infrastructure.BlockChain.BlockChainProvider.GetPublicAddress(password);
    //    }

    //    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    //    {
    //        //TODO: need to check if the password is valid hex
    //        var publicAddress = Infrastructure.BlockChain.BlockChainProvider.GetPublicAddress(providedPassword);

    //        if (hashedPassword == publicAddress)
    //        {
    //            return PasswordVerificationResult.Success;
    //        }

    //        return PasswordVerificationResult.Failed;
    //    }
    //}

    public class SbSignInManager : SignInManager<User>
    {
        public async Task<SignInResult> SignInTwoFactorAsync(User user, bool isPersistent)
        {
            var signInResult = await SignInOrTwoFactorAsync(user, isPersistent);
            if (signInResult.Succeeded)
            {
                await SignOutAsync();
                return SignInResult.Failed;
            }
            return signInResult;
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

        public SbSignInManager(UserManager<User> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<User> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<User>> logger, IAuthenticationSchemeProvider schemes) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
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
