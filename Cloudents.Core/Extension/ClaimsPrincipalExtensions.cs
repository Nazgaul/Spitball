using System;
using System.Linq;
using System.Security.Claims;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Extension
{
    public static class ClaimsPrincipalExtensions
    {
        public const string ClaimCountry = "Country";
        public const string SbClaimCountry = "SbCountry";
        private const string ClaimId = "UserId";

        public static string? GetCountryClaim(this ClaimsPrincipal principal)
        {
            var country = principal.Claims.First(w => string.Equals(w.Type, ClaimCountry, StringComparison.OrdinalIgnoreCase)).Value;
            return country != "None" ? country : null;
        }


        public static Country? GetSbCountryClaim(this ClaimsPrincipal principal)
        {
            var country = principal.Claims.First(w => string.Equals(w.Type, SbClaimCountry, StringComparison.OrdinalIgnoreCase)).Value;
            return country != "None" ? Country.FromCountry(country) : null;
        }

        public static Guid GetIdClaim(this ClaimsPrincipal principal)
        {
            return Guid.Parse(principal.Claims.First(w => string.Equals(w.Type, ClaimId, StringComparison.OrdinalIgnoreCase)).Value);
        }
    }
}