﻿using System;
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
    public class TestController : ApiController
    {
        public ApiServices Services { get; set; }

        // GET api/Test
        public string Get()
        {
            Services.Log.Info("Hello from custom controller!");
            return "Hello";
        }

        [AuthorizeLevel(AuthorizationLevel.User)]
        [HttpPost]
        public async Task<string> Facebook()
        {
            Microsoft.WindowsAzure.Mobile.Service.Security.FacebookCredentials x = new FacebookCredentials();


            var currentUser = User as ServiceUser;
            var providers = await currentUser.GetIdentitiesAsync();
            var facebookProvider = providers.OfType<FacebookCredentials>().FirstOrDefault();
            return "Hello";
        }

    }
}
