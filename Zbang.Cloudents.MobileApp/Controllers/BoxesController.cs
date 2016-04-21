using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    [Authorize]
    public class BoxesController : ApiController
    {
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IQueueProvider m_QueueProvider;
        public BoxesController(IZboxWriteService zboxWriteService, IZboxCacheReadService zboxReadService, IQueueProvider queueProvider)
        {
            m_ZboxWriteService = zboxWriteService;
            m_ZboxReadService = zboxReadService;
            m_QueueProvider = queueProvider;
        }


        //[VersionedRoute("api/boxes", 3)]
        [Route("api/boxes")]
        // ReSharper disable once ConsiderUsingAsyncSuffix - api call
        public async Task<HttpResponseMessage> GetBoxes3(int page, int sizePerPage = 15)
        {
            var userid = User.GetCloudentsUserId();
            var query = new GetBoxesQuery(userid, page, sizePerPage);
            var tData = m_ZboxReadService.GetUserBoxesAsync(query);

            var tTransaction =  m_QueueProvider.InsertMessageToTranactionAsync(
                     new StatisticsData4(null, userid, DateTime.UtcNow));

            await Task.WhenAll(tData, tTransaction);

            return Request.CreateResponse(tData.Result.Select(s => new
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

        // ReSharper disable once ConsiderUsingAsyncSuffix - api call
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
            var result = await m_ZboxReadService.GetRecommendedCoursesAsync(query);
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


        // ReSharper disable once ConsiderUsingAsyncSuffix - api call
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
            await m_ZboxWriteService.InviteSystemAsync(inviteCommand);

            return Request.CreateResponse();


        }
    }
}
