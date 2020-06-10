﻿using Cloudents.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cloudents.Web.Identity
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
    {
        public const string Country = "country";
        public const string SbCountry = "SbCountry";

        public AppClaimsPrincipalFactory(UserManager<User> userManager,
            IOptions<IdentityOptions> options) :
            base(userManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var p = await base.GenerateClaimsAsync(user);

            if (!user.PhoneNumberConfirmed) return p;
            p.AddClaim(new Claim(Country, user.Country));
            p.AddClaim(new Claim(SbCountry,user.SbCountry.Name));
            //if (user.University?.Id != null)
            //{
            //    p.AddClaim(new Claim(University, user.University.Id.ToString()));
            //}
            return p;
        }
    }


}