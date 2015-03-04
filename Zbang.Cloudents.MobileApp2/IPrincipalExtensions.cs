using System.Linq;
using System.Security.Principal;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Newtonsoft.Json;
using Zbang.Cloudents.MobileApp2.Models;

namespace Zbang.Cloudents.MobileApp2
{
    public static class PrincipalExtensions
    {
        public static long GetCloudentsUserId(this IPrincipal principal)
        {

            var login = ExtractFromClaim(principal);
            return login.CUserId;
        }

        private static CustomLoginProviderCredentials ExtractFromClaim(IPrincipal principal)
        {
            var serviceUser = (ServiceUser)principal;

            var claim = serviceUser.Claims.Single(w => w.Type == "urn:microsoft:credentials");
            var login = JsonConvert.DeserializeObject<CustomLoginProviderCredentials>(claim.Value);
            return login;
        }

        public static long? GetUniversityId(this IPrincipal principal)
        {
            var login = ExtractFromClaim(principal);
            return login.UniversityId;
        }

        public static long? GetUniversityDataId(this IPrincipal principal)
        {
            var login = ExtractFromClaim(principal);
            return login.UniversityDataId;
        }


    }
}