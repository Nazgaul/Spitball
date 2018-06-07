using System.Security.Claims;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Cloudents.Web.Identity
{
    [UsedImplicitly]
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<User,ApplicationRole>
    {
        private readonly IBlockChainErc20Service _blockChain;

        public AppClaimsPrincipalFactory(UserManager<User> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> options, IBlockChainErc20Service blockChain) :
            base(userManager, roleManager, options)
        {
            _blockChain = blockChain;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var p = await base.GenerateClaimsAsync(user).ConfigureAwait(false);

            if (user.EmailConfirmed && user.PhoneNumberConfirmed)
            {
                p.AddClaim(new Claim(ClaimsType.AuthStep, SignInStepEnum.All.ToString("D")));
                p.AddClaim(new Claim(ClaimsType.PublicKey, _blockChain.GetAddress(user.PrivateKey)));
            }
            return p;
        }
    }
}