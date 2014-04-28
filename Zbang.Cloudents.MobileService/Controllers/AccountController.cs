using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Threading.Tasks;

namespace Zbang.Cloudents.MobileService.Controllers
{
    public class AccountController : ApiController
    {
        public ApiServices Services { get; set; }

        // GET api/Account
        [AuthorizeLevel(AuthorizationLevel.User)]
        [HttpPost]
        public async Task<string> Facebook()
        {
            var currentUser = User as ServiceUser;
            var providers = await currentUser.GetIdentitiesAsync();
            var facebookCredentials = providers.OfType<FacebookCredentials>().FirstOrDefault();


            Services.Log.Info("Hello from custom controller!");
            return "Hello";
        }

        public string LogIn()
        {
            return "LogIn";
        }

        public string Register()
        {
            return "Register";
        }

    }
}
