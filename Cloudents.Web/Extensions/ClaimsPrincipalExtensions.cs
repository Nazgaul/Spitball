using System;
using System.Security.Claims;

namespace Cloudents.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var s = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (s == null)
            {
                throw new ApplicationException("claim is empty");
            }
            return long.Parse(s);
        }
    }
}
