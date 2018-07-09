using System.Security.Claims;
using Cloudents.Core.Entities.Db;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Extensions
{
    public static class UserManagerExtensions {
        public static long GetLongUserId(this UserManager<User> userManager, ClaimsPrincipal principal)
        {
            return long.Parse(userManager.GetUserId(principal));
        }
    }
}