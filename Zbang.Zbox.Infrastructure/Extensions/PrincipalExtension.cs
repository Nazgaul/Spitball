using System;
using System.Security.Claims;
using System.Security.Principal;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Zbox.Infrastructure.Extensions
{
    public static class PrincipalExtension
    {
        public static long GetUserId(this IPrincipal user, bool isAuthorize = true)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            long userId = -1;

            if (isAuthorize && !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException();
            }
            var userIdClaim = ExtractValueFromClaim(user, ClaimConst.UserIdClaim);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                if (isAuthorize)
                {
                    throw new UnauthorizedAccessException();
                }
                return userId;
            }
            long.TryParse(userIdClaim, out userId);

            return userId;
        }

        private static string ExtractValueFromClaim(IPrincipal user, string claimType)
        {
            var principal = (ClaimsPrincipal)user;
            var claim = principal.FindFirst(claimType);
            return claim?.Value;
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