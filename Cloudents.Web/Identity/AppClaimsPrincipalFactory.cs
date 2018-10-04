using System.Security.Claims;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Cloudents.Web.Identity
{
    [UsedImplicitly]
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, ApplicationRole>
    {
        internal const string Country = "country";
        internal const string University = "university";

        public AppClaimsPrincipalFactory(UserManager<User> userManager, RoleManager<ApplicationRole> roleManager, 
            IOptions<IdentityOptions> options) :
            base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var p = await base.GenerateClaimsAsync(user).ConfigureAwait(false);

            if (user.EmailConfirmed && user.PhoneNumberConfirmed)
            {
                p.AddClaim(new Claim(Country, user.Country));
                p.AddClaim(new Claim(University, user.University?.Id.ToString()));
            }
            return p;
        }
    }
}