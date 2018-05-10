using System.Security.Claims;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Cloudents.Web.Identity
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<User,ApplicationRole>
    {
        public AppClaimsPrincipalFactory(UserManager<User> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> options) :
            base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var p = await base.GenerateClaimsAsync(user).ConfigureAwait(false);
            var step = (user.EmailConfirmed ? SigninStep.Email :
                           SigninStep.None)
                |
                (user.PhoneNumberConfirmed ? SigninStep.Sms : SigninStep.None);
            p.AddClaim(new Claim(SignInStep.Claim, step.ToString("D")));
            return p;
        }
       

    }
}