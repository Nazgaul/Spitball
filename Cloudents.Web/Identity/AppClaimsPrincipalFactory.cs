using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cloudents.Web.Identity
{
    [UsedImplicitly]
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, ApplicationRole>
    {
        internal const string Country = "country";
        internal const string University = "university";
        internal const string Languages = "languages";

        public AppClaimsPrincipalFactory(UserManager<User> userManager, RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> options) :
            base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var p = await base.GenerateClaimsAsync(user).ConfigureAwait(false);

            if (!user.EmailConfirmed || !user.PhoneNumberConfirmed) return p;
            p.AddClaim(new Claim(Country, user.Country));
            if (user.University?.Id != null)
            {
                p.AddClaim(new Claim(University, user.University.Id.ToString()));
            }

            //if (user.Languages != null)
            //{
            //    p.AddClaim(new Claim(Languages, string.Join(",", user.Languages)));
            //    //foreach (var language in user.Languages)
            //    //{
            //    //    p.AddClaim(new Claim(Languages, language.ToString()));

            //    //}
            //}
            return p;
        }
    }
}