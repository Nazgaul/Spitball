using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Zbang.Cloudents.MobileApp2
{
    public static class IPrincipalExtensions
    {
        public static long GetCloudentsUserId(this IPrincipal principal)
        {
            return 1;
            var serviceUser = (Microsoft.WindowsAzure.Mobile.Service.Security.ServiceUser) principal;
            var stringUserId = serviceUser.Id.Replace("cloudents:", string.Empty);
            return long.Parse(stringUserId);
        }
    }
}