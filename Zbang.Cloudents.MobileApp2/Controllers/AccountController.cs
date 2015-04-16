using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class AccountController : ApiController
    {
        public ApiServices Services { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }

        public IZboxWriteService ZboxWriteService { get; set; }

        public IServiceTokenHandler Handler { get; set; }

        public ApplicationUserManager UserManager { get; set; }
        public IQueueProvider QueueProvider { get; set; }

        // GET api/Account
        [HttpGet]
        [Route("api/account/details")]
        public async Task<HttpResponseMessage> Details()
        {
            var retVal = await ZboxReadService.GetUserDataAsync(new GetUserDetailsQuery(User.GetCloudentsUserId()));
            return Request.CreateResponse(new
            {
                retVal.Id,
                retVal.UniversityId,
                retVal.Name,
                retVal.Image,
                retVal.IsAdmin,
                //retVal.FirstTimeDashboard,
                //retVal.Score,
                retVal.UniversityCountry,
                retVal.UniversityName,
                tokenValid = retVal.UniversityId == User.GetUniversityId()
            });
        }

        [HttpGet]
        [Route("api/account/tokenrefresh")]
        public async Task<HttpResponseMessage> Refresh()
        {

            var systemData = await ZboxReadService.GetUserDetailsById(new GetUserByIdQuery(User.GetCloudentsUserId()));

            if (!systemData.UniversityId.HasValue)
            {
                return Request.CreateBadRequestResponse("user don't have university");
            }
            
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimConsts.UserIdClaim, User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)));


            identity.AddClaim(new Claim(ClaimConsts.UniversityIdClaim,
                    systemData.UniversityId.Value.ToString(CultureInfo.InvariantCulture)));

            identity.AddClaim(new Claim(ClaimConsts.UniversityDataClaim,
                    systemData.UniversityData.HasValue ?
                    systemData.UniversityData.Value.ToString(CultureInfo.InvariantCulture)
                    : systemData.UniversityId.Value.ToString(CultureInfo.InvariantCulture)));
            var loginResult = new Models.CustomLoginProvider(Handler)
                    .CreateLoginResult(identity, Services.Settings.MasterKey);
            return Request.CreateResponse(HttpStatusCode.OK, loginResult);
        }

        [HttpGet]
        [Route("api/account/university/russianDepartments")]
        public async Task<HttpResponseMessage> RussianDepartments()
        {
            var retVal = await ZboxReadService.GetRussianDepartmentList(984);
            return Request.CreateResponse(retVal);
        }

        [HttpPost]
        [Route("api/account/university")]
        public async Task<HttpResponseMessage> UpdateUniversity(UpdateUniversityRequest model)
        {
            
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            //var retVal = await ZboxReadService.GetRussianDepartmentList(model.UniversityId);
            //if (retVal.Count() != 0 && !model.DepartmentId.HasValue)
            //{
            //    return Request.CreateResponse(0);
            //}
            var needId = await ZboxReadService.GetUniversityNeedId(model.UniversityId);
            if (needId && string.IsNullOrEmpty(model.StudentId))
            {
                return Request.CreateResponse(1);
                //return RedirectToAction("InsertId", "Library", new { universityId = model.UniversityId });
            }

            var needCode = await ZboxReadService.GetUniversityNeedCode(model.UniversityId);
            if (needCode && string.IsNullOrEmpty(model.Code))
            {
                return Request.CreateResponse(2);
                //return RedirectToAction("InsertCode", "Library", new { universityId = model.UniversityId });
            }

            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var id = User.GetCloudentsUserId();
            var command = new UpdateUserUniversityCommand(model.UniversityId, id,null, model.Code, null,
                null, model.StudentId);
            try
            {
                ZboxWriteService.UpdateUserUniversity(command);
            }
            catch (ArgumentException ex)
            {
                return Request.CreateBadRequestResponse(ex.Message);
            }


            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimConsts.UserIdClaim, User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)));


            identity.AddClaim(new Claim(ClaimConsts.UniversityIdClaim,
                    command.UniversityId.ToString(CultureInfo.InvariantCulture)));

            identity.AddClaim(new Claim(ClaimConsts.UniversityDataClaim,
                    command.UniversityDataId.HasValue ?
                    command.UniversityDataId.Value.ToString(CultureInfo.InvariantCulture)
                    : command.UniversityId.ToString(CultureInfo.InvariantCulture)));


            var loginResult = new Models.CustomLoginProvider(Handler)
                    .CreateLoginResult(identity, Services.Settings.MasterKey);


            return Request.CreateResponse(HttpStatusCode.OK, loginResult);

        }


        [HttpPost]
        [Route("api/account/resetPassword")]
        [AuthorizeLevel(AuthorizationLevel.Application)]
        public async Task<HttpResponseMessage> ResetPassword(ResetPasswordRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var query = new GetUserByEmailQuery(model.Email);
            var tUser = UserManager.FindByEmailAsync(model.Email);
            var tResult = ZboxReadService.GetUserDetailsByEmail(query);

            await Task.WhenAll(tUser, tResult);
            if (tUser.Result == null && tResult.Result != null)
            {
                return Request.CreateBadRequestResponse("You have registered to Cloudents through Facebook -- go to the homepage and click on the Facebook button to register");
            }
            if (tUser.Result == null)
            {
                return Request.CreateBadRequestResponse("email doesn't exists");
            }
            if (tResult.Result == null)
            {
                return Request.CreateBadRequestResponse();
            }
            var user = tUser.Result;
            var identitylinkData = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var code = RandomString(4);
            await QueueProvider.InsertMessageToMailNewAsync(new ForgotPasswordData2(code, identitylinkData, tResult.Result.Name.Split(' ')[0], model.Email, tResult.Result.Culture));

            return Request.CreateResponse(new { code, resetToken = identitylinkData, user.Id });
        }

        [HttpPost]
        [Route("api/account/passwordUpdate")]
        [AuthorizeLevel(AuthorizationLevel.Application)]
        public async Task<HttpResponseMessage> PasswordUpdate(PasswordUpdateRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var result = await UserManager.ResetPasswordAsync(model.UserId, model.ResetToken, model.NewPassword);

            if (!result.Succeeded)
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    new HttpError("something went wrong, try again later"));
            var query = new GetUserByMembershipQuery(Guid.Parse(model.UserId));
            var tSystemUser = ZboxReadService.GetUserDetailsByMembershipId(query);

            var tUser = UserManager.FindByIdAsync(model.UserId);
            await Task.WhenAll(tSystemUser, tUser);

            var identity = await tUser.Result.GenerateUserIdentityAsync(UserManager, tSystemUser.Result.Id,
                tSystemUser.Result.UniversityId, tSystemUser.Result.UniversityData);
            var loginResult = new Models.CustomLoginProvider(Handler)
                .CreateLoginResult(identity, Services.Settings.MasterKey);
            return Request.CreateResponse(HttpStatusCode.OK, loginResult);
        }



        private static string RandomString(int size)
        {
            var random = new Random();
            const string input = "0123456789";
            var chars = Enumerable.Range(0, size)
                                   .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray());
            //return "12345";
        }

    }
}
