using System.Security.Claims;
using Cloudents.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Extensions
{
    public static class UserManagerExtensions {
        public static long GetLongUserId(this UserManager<RegularUser> userManager, ClaimsPrincipal principal)
        {
            return long.Parse(userManager.GetUserId(principal));
        }
    }
}