using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    [Authorize]
    public class UserController : ApiController
    {
        private readonly IZboxCacheReadService m_ZboxReadService;

        public UserController(IZboxCacheReadService zboxReadService)
        {
            m_ZboxReadService = zboxReadService;
        }

        [HttpGet]
        [Route("api/user/{userId}/boxes")]
        public async Task<HttpResponseMessage> BoxesAsync(long userId, int page, int sizePerPage = 15)
        {
            var query = new GetUserWithFriendQuery(userId, page, sizePerPage);
            var model = await m_ZboxReadService.GetUserBoxesActivityAsync(query);
            return Request.CreateResponse(model.Select(s => new
            {
                s.Id,
                s.Name,
                s.Professor,
                s.CourseCode,
                s.ItemCount,
                s.CommentCount,
                s.BoxType,
                shortUrl = UrlConst.BuildShortBoxUrl(new Base62(s.Id).ToString())
            }));
        }

        [Route("api/user/{userId}")]
        public async Task<HttpResponseMessage> GetAsync(long userId)
        {
            var query = new GetUserMinProfileQuery(userId);
            var result = await m_ZboxReadService.GetUserMinProfileAsync(query);
            return Request.CreateResponse(new
            {
                result.Id,
                result.Name,
                result.UniversityName,
                result.Image,
                result.Score
                
            });

        }

        [HttpGet]
        [Route("api/user/{userId}/friends")]
        public async Task<HttpResponseMessage> FriendsAsync(long userId, int page, int sizePerPage = 15)
        {
            var query = new GetUserFriendsQuery(userId, page, sizePerPage);
            var result = await m_ZboxReadService.GetUserFriendsAsync(query);

            return Request.CreateResponse(result.Select(s => new
            {
                s.Id,
                s.Name, s.Image
            }));
        }

        [HttpGet]
        [Route("api/user/{userId}/activity")]
        public async Task<HttpResponseMessage> ActivityAsync(long userId, int page, int sizePerPage = 15)
        {
            var query = new GetUserWithFriendQuery( userId, page, sizePerPage);
            var result = await m_ZboxReadService.GetUserCommentActivityAsync(query);

            return Request.CreateResponse(result.Select( s=> new
            {
                s.BoxId,
                s.BoxName,
                s.Content,
                s.Id,
                s.PostId,
                s.Type
            }));
        }


        [HttpGet]
        [Route("api/user/{userId}/items")]
        public async Task<HttpResponseMessage> ItemsAsync(long userId, int page, int sizePerPage = 15)
        {
            var query = new GetUserWithFriendQuery(userId, page, sizePerPage);
            var result = await m_ZboxReadService.GetUserItemsActivityAsync(query);

            return Request.CreateResponse(result.Select(s => new
            {
                s.Id,
                s.BoxId, s.Source,
                s.Name,
                s.Type
            }));

        }
    }
}
