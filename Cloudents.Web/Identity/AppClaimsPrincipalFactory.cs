using Cloudents.Core.Entities;
using Cloudents.Query;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cloudents.Web.Identity
{
    [UsedImplicitly]
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
    {
        public const string Country = "country";
        public const string University = "university";
        public const string Score = "score";
        //internal const string Profile = "profile";

        public AppClaimsPrincipalFactory(UserManager<User> userManager,
            //RoleManager<ApplicationRole> roleManager,
            IQueryBus queryBus,
            IOptions<IdentityOptions> options) :
            base(userManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var p = await base.GenerateClaimsAsync(user);

            if (!user.PhoneNumberConfirmed) return p;
            p.AddClaim(new Claim(Country, user.Country));
            p.AddClaim(new Claim(Score, user.Transactions.Score.ToString()));
            if (user.University?.Id != null)
            {
                p.AddClaim(new Claim(University, user.University.Id.ToString()));
            }

            //var query = new UserDataQuery(user.Id);
            //var result = await _queryBus.QueryAsync(query, default);
            //var v = JsonConvert.SerializeObject(result);
            //if (v.Length < 2000)
            //{
            //    p.AddClaim(new Claim(Profile, JsonConvert.SerializeObject(result)));
            //}

            return p;
        }
    }


}