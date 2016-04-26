using System.Security.Claims;
using System.Security.Principal;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Cloudents.MobileApp.Extensions
{
    public static class PrincipalExtensions
    {
        public static long GetCloudentsUserId(this IPrincipal user)
        {
            long userId = -1;
            
            var userIdClaim = ExtractValueFromClaim(user, ClaimConst.UserIdClaim);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return userId;
            }
            long.TryParse(userIdClaim, out userId);

            return userId;
        }

        private static string ExtractValueFromClaim(IPrincipal user, string claimType)
        {
            var principal = (ClaimsPrincipal)user;
            var claim = principal.FindFirst(claimType);
            if (claim == null)
            {
                return null;
            }
            return claim.Value;
        }

        public static long? GetUniversityId(this IPrincipal user)
        {
            long value;
            var claim = ExtractValueFromClaim(user, ClaimConst.UniversityIdClaim);
            if (claim == null)
            {
                return null;
            }
            if (long.TryParse(claim, out value))
            {
                return value;
            }
            return null;


        }

        public static long? GetUniversityDataId(this IPrincipal user)
        {
            long value;
            var claim = ExtractValueFromClaim(user, ClaimConst.UniversityDataClaim);
            if (claim == null)
            {
                return null;
            }
            if (long.TryParse(claim, out value))
            {
                return value;
            }
            return null;
        }
       

       

        


    }
}