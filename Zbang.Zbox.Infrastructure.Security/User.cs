using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class User : IdentityUser
    {
       

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new Claim(ClaimConsts.UserIdClaim, UserId.ToString(CultureInfo.InvariantCulture)));
            if (UniversityId.HasValue && UniversityData.HasValue)
            {
                userIdentity.AddClaim(new Claim(ClaimConsts.UniversityIdClaim, UniversityId.Value.ToString(CultureInfo.InvariantCulture)));
                userIdentity.AddClaim(new Claim(ClaimConsts.UniversityDataClaim, UniversityData.Value.ToString(CultureInfo.InvariantCulture)));
            }
            // Add custom user claims here
            return userIdentity;
        }
        [NotMapped]
        public long UserId { get; set; }
        [NotMapped]
        public long? UniversityId { get; set; }
        [NotMapped]
        public long? UniversityData { get; set; }



    }
}
