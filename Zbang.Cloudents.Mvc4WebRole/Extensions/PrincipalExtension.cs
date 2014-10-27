using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
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