using System;
using Cloudents.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Cloudents.Web.Extensions
{
    public static class UserManagerExtensions
    {
        public static long GetLongUserId(this UserManager<User> userManager, ClaimsPrincipal principal)
        {
            if (userManager == null) throw new ArgumentNullException(nameof(userManager));
            return long.Parse(userManager.GetUserId(principal));
        }

        public static bool TryGetLongUserId(this UserManager<User> userManager, ClaimsPrincipal principal, out long userId)
        {
            userId = 0;
            if (!principal.Identity.IsAuthenticated)
            {
                return false;
            }
            userId = GetLongUserId(userManager, principal);
            return true;
        }
    }
}