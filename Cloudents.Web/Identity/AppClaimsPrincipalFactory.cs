using System.Security.Claims;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Cloudents.Web.Identity
{
    [UsedImplicitly]
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<User,ApplicationRole>
    {
        public AppClaimsPrincipalFactory(UserManager<User> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> options) :
            base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var p = await base.GenerateClaimsAsync(user).ConfigureAwait(false);
            var step = (user.EmailConfirmed ? SignInStepEnum.Email :
                           SignInStepEnum.None)
                |
                (user.PhoneNumberConfirmed ? SignInStepEnum.Sms : SignInStepEnum.None)
                | (!string.IsNullOrEmpty(user.PublicKey) ? SignInStepEnum.Password : SignInStepEnum.None);
            p.AddClaim(new Claim(SignInStep.Claim, step.ToString("D")));
            return p;
        }

    }
}