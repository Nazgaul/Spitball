using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Caching;

namespace Zbang.Zbox.MvcWebRole.Helpers
{
    public static class ExtractUserID
    {
        const string kCacheKey = "UserName";
        public static string GetUserEmailId(string userName, bool isAuthorize)       
        {
            MembershipUser membershipUser = Membership.GetUser(userName);
            if (!isAuthorize && membershipUser == null)
                return string.Empty;

            return membershipUser.Email.ToLower();
        }
        


        public static string GetUserEmailId(bool isAuthorize = true)
        {

            return GetUserEmailId(HttpContext.Current.User.Identity.Name, isAuthorize);
        }
    }
}