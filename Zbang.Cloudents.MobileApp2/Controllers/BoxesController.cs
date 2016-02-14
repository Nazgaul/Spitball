using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Cloudents.MobileApp2.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class BoxesController : ApiController
    {
        public ApiServices Services { get; set; }

        public IZboxWriteService ZboxWriteService { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }

        // GET api/Boxes
        [VersionedRoute("api/boxes", 1)]
        public async Task<HttpResponseMessage> Get(int page, int sizePerPage = 15)
        {
            var userid = User.GetCloudentsUserId();
            var query = new GetBoxesQuery(userid, page, sizePerPage);
            var data = await ZboxReadService.GetUserBoxesOld(query);

            return Request.CreateResponse(data.Where(w=>w.BoxType != Zbox.Infrastructure.Enums.BoxType.AcademicClosed).Select(s => new
            {
                s.Name,
                s.Id,
                s.ItemCount,
                s.CommentCount,
                s.Updates,
                s.BoxType,
                s.UserType,
                s.Professor,
                s.CourseCode,
                s.Url,
                shortUrl = UrlConsts.BuildShortBoxUrl(new Base62(s.Id).ToString())
            }));
        }

        [VersionedRoute("api/boxes", 2)]
        public async Task<HttpResponseMessage> GetBoxes(int page, int sizePerPage = 15)
        {
            var userid = User.GetCloudentsUserId();
            var query = new GetBoxesQuery(userid, page, sizePerPage);
            var data = await ZboxReadService.GetUserBoxesAsync(query);

            return Request.CreateResponse(data.Where(w => w.BoxType != Zbox.Infrastructure.Enums.BoxType.AcademicClosed).Select(s => new
            {
                s.Name,
                s.Id,
                s.ItemCount,
                s.CommentCount,
                s.BoxType,
                s.UserType,
                s.Professor,
                s.CourseCode,
                shortUrl = UrlConsts.BuildShortBoxUrl(new Base62(s.Id).ToString())
            }));
        }

        [HttpGet]
        [Route("api/boxes/recommend")]
        public async Task<HttpResponseMessage> Recommend()
        {
            var university = User.GetUniversityDataId();
            if (!university.HasValue)
            {
                return Request.CreateBadRequestResponse("user don't have university");
            }
            // ReSharper disable once PossibleInvalidOperationException - universityid have value because no university attribute
            //var universityWrapper = userDetail.UniversityDataId.Value;

            var query = new RecommendedCoursesQuery(university.Value, User.GetCloudentsUserId());
            var result = await ZboxReadService.GetRecommendedCoursesAsync(query);
            return Request.CreateResponse(result.Select(s => new
            {
                Id = s.BoxId,
                s.Professor,
                s.CourseCode,
                s.Name,
                s.MembersCount,
                s.ItemCount,
                shortUrl = UrlConsts.BuildShortBoxUrl(new Base62(s.BoxId).ToString())
                //this is always a course
            }));
        }



        [HttpPost, Route("api/invite")]
        public async Task<HttpResponseMessage> Invite(InviteToSystemRequest model)
        {

            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var userId = User.GetCloudentsUserId();

            var inviteCommand = new InviteToSystemCommand(userId, model.Recipients);
            await ZboxWriteService.InviteSystemAsync(inviteCommand);

            return Request.CreateResponse();


        }
    }
}
