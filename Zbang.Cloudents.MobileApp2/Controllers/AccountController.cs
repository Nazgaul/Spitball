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
                retVal.FirstTimeDashboard,
                retVal.Score,
                retVal.UniversityCountry,
                retVal.UniversityName,
            });
        }

        [HttpPost]
        [Route("api/account/university")]
        public async Task<HttpResponseMessage> UpdateUniversity(UpdateUniversityRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            var retVal = await ZboxReadService.GetRussianDepartmentList(model.UniversityId);
            if (retVal.Count() != 0 && !model.DepartmentId.HasValue)
            {
                return Request.CreateResponse(0);
            }
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
            var command = new UpdateUserUniversityCommand(model.UniversityId, id, null, null,
                null, null, null);
            ZboxWriteService.UpdateUserUniversity(command);


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

    }
}
