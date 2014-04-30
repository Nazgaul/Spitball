using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Security;


namespace Zbang.Cloudents.MobileService.Controllers
{
    public class AccountController : ApiController
    {
        public ApiServices Services { get; set; }
        public IFacebookAuthenticationService FacebookService { get; set; }

        // GET api/Account
        [AuthorizeLevel(AuthorizationLevel.User)]
        [HttpPost]
        public async Task<string> Facebook()
        {
            var currentUser = User as ServiceUser;
            var providers = await currentUser.GetIdentitiesAsync();
            var facebookCredentials = providers.OfType<FacebookCredentials>().FirstOrDefault();

            var facebookData = await FacebookService.FacebookLogIn(facebookCredentials.AccessToken);

            Services.Log.Info(facebookData.id.ToString());
            return "Hello";
        }

        public LoginResult LogIn()
        {
            return new LoginResult()
            {

                User = new LoginResultUser { UserId = "1" },
                AuthenticationToken = "blabla"
            };
           // return LoginResult;
        }

        public string Register()
        {
            return "Register";
        }

    }
}
