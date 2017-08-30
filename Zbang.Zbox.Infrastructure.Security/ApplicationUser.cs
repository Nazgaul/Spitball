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
    public class ApplicationUser : IdentityUser
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, long userId, long? universityId, long? universityDataId)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return AddClaims(userId, universityId, universityDataId, userIdentity);
        }

        private static ClaimsIdentity AddClaims(long userId, long? universityId, long? universityDataId, ClaimsIdentity userIdentity)
        {
            userIdentity.AddClaim(new Claim(ClaimConst.UserIdClaim, userId.ToString(CultureInfo.InvariantCulture)));
            if (universityId.HasValue && universityDataId.HasValue)
            {
                userIdentity.AddClaim(new Claim(ClaimConst.UniversityIdClaim, universityId.Value.ToString(CultureInfo.InvariantCulture)));
                userIdentity.AddClaim(new Claim(ClaimConst.UniversityDataClaim, universityDataId.Value.ToString(CultureInfo.InvariantCulture)));
            }
            return userIdentity;
        }

        public static ClaimsIdentity GenerateUserIdentity(long userId, long? universityId, long? universityDataId)
        {
            var userIdentity =  new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimConst.UserIdClaim, userId.ToString(CultureInfo.InvariantCulture)),

                },
                   DefaultAuthenticationTypes.ApplicationCookie);
            return AddClaims(userId, universityId, universityDataId, userIdentity);
        }



    }
}
