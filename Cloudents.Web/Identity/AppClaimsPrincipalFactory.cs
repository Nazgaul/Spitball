using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Query;
using Newtonsoft.Json;

namespace Cloudents.Web.Identity
{
    [UsedImplicitly]
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<RegularUser, ApplicationRole>
    {
        private readonly IQueryBus _queryBus;
        internal const string Country = "country";
        internal const string University = "university";
        internal const string Score = "score";
        internal const string Profile = "profile";

        public AppClaimsPrincipalFactory(UserManager<RegularUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IQueryBus queryBus,
            IOptions<IdentityOptions> options) :
            base(userManager, roleManager, options)
        {
            _queryBus = queryBus;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(RegularUser user)
        {
            var p = await base.GenerateClaimsAsync(user);

            if (!user.EmailConfirmed || !user.PhoneNumberConfirmed) return p;
            p.AddClaim(new Claim(Country, user.Country));
            p.AddClaim(new Claim(Score, user.Transactions.Score.ToString()));
            if (user.University?.Id != null)
            {
                p.AddClaim(new Claim(University, user.University.Id.ToString()));
            }

            var query = new UserProfileQuery(user.Id);
            var result = await _queryBus.QueryAsync(query, default);

            p.AddClaim(new Claim(Profile, JsonConvert.SerializeObject(result)));

            return p;
        }
    }

    
}