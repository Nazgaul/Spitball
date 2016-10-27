using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Cloudents.MobileApp.Filters;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    [ZboxAuthorize]
    public class BoxesController : ApiController
    {
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IZboxCacheReadService m_ZboxReadService;
        public BoxesController(IZboxWriteService zboxWriteService, IZboxCacheReadService zboxReadService)
        {
            m_ZboxWriteService = zboxWriteService;
            m_ZboxReadService = zboxReadService;
        }

        
        //[VersionedRoute("api/boxes", 3)]
        
        [ZboxAuthorize]
        [Route("api/boxes", Order = 3)]
        public async Task<HttpResponseMessage> GetBoxesAsync(int page, int sizePerPage = 15)
        {
            var userid = User.GetUserId();
            var query = new GetBoxesQuery(userid, page, sizePerPage);
            var data = await m_ZboxReadService.GetUserBoxesAsync(query);
            return Request.CreateResponse(data.Select(s => new
            {
                s.Name,
                s.Id,
                s.ItemCount,
                s.CommentCount,
                s.BoxType,
                s.UserType,
                s.Professor,
                s.CourseCode,
                shortUrl = UrlConst.BuildShortBoxUrl(new Base62(s.Id).ToString())
            }));
        }

        [HttpGet]
        [Route("api/boxes/recommend")]
        public async Task<HttpResponseMessage> Recommend()
        {
            return Request.CreateResponse();
            //if (!User.Identity.IsAuthenticated)
            //{
            //    return Request.CreateUnauthorizedResponse();
            //}
            //var university = User.GetUniversityDataId();
            //if (!university.HasValue)
            //{
            //    return Request.CreateBadRequestResponse("user don't have university");
            //}
            
            //var query = new RecommendedCoursesQuery(university.Value, User.GetUserId());
            //var result = await m_ZboxReadService.GetRecommendedCoursesAsync(query);
            //return Request.CreateResponse(result.Select(s => new
            //{
            //    Id = s.BoxId,
            //    s.Professor,
            //    s.CourseCode,
            //    s.Name,
            //    s.MembersCount,
            //    s.ItemCount,
            //    shortUrl = UrlConst.BuildShortBoxUrl(new Base62(s.BoxId).ToString())
            //    //this is always a course
            //}));
        }


        // ReSharper disable once ConsiderUsingAsyncSuffix - api call
        [HttpPost, Route("api/invite")]
        public async Task<HttpResponseMessage> Invite(InviteToSystemRequest model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Request.CreateUnauthorizedResponse();
            }
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var userId = User.GetUserId();

            var inviteCommand = new InviteToSystemCommand(userId, model.Recipients);
            await m_ZboxWriteService.InviteSystemAsync(inviteCommand);

            return Request.CreateResponse();


        }
    }
}
