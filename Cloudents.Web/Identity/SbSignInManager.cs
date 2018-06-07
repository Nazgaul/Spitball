using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            var t = await SignInOrTwoFactorAsync(user, isPersistent);
            return t;
        }

        public SbSignInManager(UserManager<User> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<User> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<User>> logger, IAuthenticationSchemeProvider schemes) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }
    }
}
