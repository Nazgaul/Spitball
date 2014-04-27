using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace Zbang.Cloudents.MobileService.Controllers
{
    public class TestController : ApiController
    {
	    public ApiServices Services { get; set; }


		// GET api/Test
        [AuthorizeLevel(Microsoft.WindowsAzure.Mobile.Service.Security.AuthorizationLevel.User)]
        public string Get()
        {

            var user = User as ServiceUser;
            
            Services.Log.Info("Hello from custom controller!");
            return "Hello";
        }

    }
}
