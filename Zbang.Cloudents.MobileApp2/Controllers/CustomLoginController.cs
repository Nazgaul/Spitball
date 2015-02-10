using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class CustomLoginController : ApiController
    {
        public ApiServices Services { get; set; }
        public IServiceTokenHandler Handler { get; set; }


        public IZboxReadService ZboxReadService { get; set; }


        public UserManager UserManager
        {
            get
            {
                return  HttpContext.Current.GetOwinContext().GetUserManager<UserManager>();
            }
        }
        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return  HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //}

        // GET api/CustomLogin
        public async Task<HttpResponseMessage> Post(LogInRequest loginRequest)
        {
            

            if (!ModelState.IsValid || loginRequest == null)
            {
                return Request.CreateBadRequestResponse();
            }
            try
            {
                var query = new GetUserByEmailQuery(loginRequest.Email);
                var tSystemData = ZboxReadService.GetUserDetailsByEmail(query);
                var tUserIdentity = UserManager.FindByEmailAsync(loginRequest.Email);

                await Task.WhenAll(tSystemData, tUserIdentity);

                var user = tUserIdentity.Result;
                if (user == null)
                {
                    return Request.CreateBadRequestResponse();
                }
                var systemUser = tSystemData.Result;
                if (systemUser == null)
                {
                    return Request.CreateBadRequestResponse();
                }
                user.UserId = systemUser.Id;
                user.UniversityId = systemUser.UniversityId;
                user.UniversityData = systemUser.UniversityData;

                if (await UserManager.CheckPasswordAsync(user,loginRequest.Password))
                {
                    var claimsIdentity = new ClaimsIdentity();
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)));
                    claimsIdentity.AddClaim(new Claim(ClaimConsts.UserIdClaim, user.UserId.ToString(CultureInfo.InvariantCulture)));
                    if (user.UniversityId.HasValue && user.UniversityData.HasValue)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimConsts.UniversityIdClaim, user.UniversityId.Value.ToString(CultureInfo.InvariantCulture)));
                        claimsIdentity.AddClaim(new Claim(ClaimConsts.UniversityDataClaim, user.UniversityData.Value.ToString(CultureInfo.InvariantCulture)));
                    }
                    var loginResult = new Models.CustomLoginProvider(Handler).CreateLoginResult(claimsIdentity, Services.Settings.MasterKey);
                    return Request.CreateResponse(HttpStatusCode.OK, loginResult);
                }
                //Guid membershipUserId;
                
                //var loginStatus = MembershipService
                //    .ValidateUser(loginRequest.Email, loginRequest.Password, out membershipUserId);
                //if (loginStatus == LogInStatus.Success)
                //{
                //    try
                //    {
                //        var query = new GetUserByMembershipQuery(membershipUserId);
                //        var result = await ZboxReadService.GetUserDetailsByMembershipId(query);
                        
                //    }
                //    catch (UserNotFoundException)
                //    {
                //        return Request.CreateBadRequestResponse();
                //        //ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
                //    }
                //}
                //else
                //{
                //    return Request.CreateBadRequestResponse();
                //    //ModelState.AddModelError(string.Empty, loginStatus.GetEnumDescription());
                //}

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
