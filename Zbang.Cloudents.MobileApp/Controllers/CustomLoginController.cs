using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Cloudents.MobileApp.Models;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class CustomLoginController : ApiController
    {
        public ApiServices Services { get; set; }
        public IServiceTokenHandler Handler { get; set; }

        public IMembershipService MembershipService { get; set; }

        public IZboxReadService ZboxReadService { get; set; }

        // GET api/CustomLogin
        public async Task<HttpResponseMessage> Post(LogInRequest loginRequest)
        {
            if (!ModelState.IsValid || loginRequest == null)
            {
                return Request.CreateBadRequestResponse();
            }
            try
            {
                Guid membershipUserId;
                var loginStatus = MembershipService
                    .ValidateUser(loginRequest.Email, loginRequest.Password, out membershipUserId);
                if (loginStatus == LogInStatus.Success)
                {
                    try
                    {
                        var query = new GetUserByMembershipQuery(membershipUserId);
                        var result = await ZboxReadService.GetUserDetailsByMembershipId(query);
                        var claimsIdentity = new ClaimsIdentity();
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.Id.ToString(CultureInfo.InvariantCulture)));
                        var loginResult = new CustomLoginProvider(Handler).CreateLoginResult(claimsIdentity, Services.Settings.MasterKey);
                        return Request.CreateResponse(HttpStatusCode.OK, loginResult);
                    }
                    catch (UserNotFoundException)
                    {
                        return Request.CreateBadRequestResponse();
                        //ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
                    }
                }
                else
                {
                    return Request.CreateBadRequestResponse();
                    //ModelState.AddModelError(string.Empty, loginStatus.GetEnumDescription());
                }

            }
            catch (Exception ex)
            {

                Services.Log.Error(string.Format("LogOn model : {0} ", loginRequest), ex);
                //ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
            }
            return Request.CreateBadRequestResponse();
           
        }

    }
}
