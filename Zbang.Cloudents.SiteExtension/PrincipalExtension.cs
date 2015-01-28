using System;
using System.Security.Principal;

namespace Zbang.Cloudents.SiteExtension
{
    public static class PrincipalExtension
    {
        public static long GetUserId(this IPrincipal user, bool isAuthorize = true)
        {
            long userId;
            
            if (isAuthorize && string.IsNullOrEmpty(user.Identity.Name))
            {
                throw new UnauthorizedAccessException();
            }
            long.TryParse(user.Identity.Name, out userId);

            return userId;
        }
    }
}