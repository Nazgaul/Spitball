using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Web.Identity
{
    [UsedImplicitly]
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<RegularUser, ApplicationRole>
    {
        internal const string Country = "country";
        internal const string University = "university";
        internal const string Score = "score";

        public AppClaimsPrincipalFactory(UserManager<RegularUser> userManager, RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> options) :
            base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(RegularUser user)
        {
            var p = await base.GenerateClaimsAsync(user).ConfigureAwait(false);

            if (!user.EmailConfirmed || !user.PhoneNumberConfirmed) return p;
            p.AddClaim(new Claim(Country, user.Country));
            p.AddClaim(new Claim(Score, user.Transactions.Score.ToString()));
            if (user.University?.Id != null)
            {
                p.AddClaim(new Claim(University, user.University.Id.ToString()));
            }

            return p;
        }
    }

    
}