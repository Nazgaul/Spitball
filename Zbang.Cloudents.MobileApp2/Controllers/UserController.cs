using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class UserController : ApiController
    {
        public IZboxCacheReadService ZboxReadService { get; set; }
        [HttpGet]
        [Route("api/user/{userId}/boxes")]
        public async Task<HttpResponseMessage> Boxes(long userId, int page, int sizePerPage = 15)
        {
            var query = new GetUserWithFriendQuery(User.GetCloudentsUserId(), userId, page, sizePerPage);
            var model = await ZboxReadService.GetUserWithFriendBoxes(query);
            return Request.CreateResponse(model.Select(s => new
            {
                s.Id,
                s.Name,
                s.Professor,
                s.CourseCode,
                s.ItemCount,
                s.CommentCount,
                s.BoxType,
                shortUrl = UrlConsts.BuildShortBoxUrl(new Base62(s.Id).ToString())
            }));
        }

        [Route("api/user/{userId}")]
        public async Task<HttpResponseMessage> Get(long userId)
        {
            var query = new GetUserMinProfileQuery(userId);
            var result = await ZboxReadService.GetUserMinProfile(query);
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
        public async Task<HttpResponseMessage> Friends(long userId, int page, int sizePerPage = 15)
        {
            var query = new GetUserFriendsQuery(userId, page, sizePerPage);
            var result = await ZboxReadService.GetUserFriends(query);

            return Request.CreateResponse(result.Select(s => new
            {
                s.Id,
                s.Name,
                Image = s.LargeImage
            }));
        }

        [HttpGet]
        [Route("api/user/{userId}/activity")]
        public async Task<HttpResponseMessage> Activity(long userId, int page, int sizePerPage = 15)
        {
            var query = new GetUserWithFriendQuery(User.GetCloudentsUserId(), userId, page, sizePerPage);
            var result = await ZboxReadService.GetUserWithFriendActivityAsync(query);

            return Request.CreateResponse(new
            {
                Answers = result.Answers.Select(s => new
                 {
                     s.BoxId,
                     s.Content,
                     s.BoxName,
                     s.Id
                 }),
                Questions = result.Questions.Select(s => new
                 {
                     s.BoxId,
                     s.Content,
                     s.BoxName,
                     s.Id
                 })
            });
        }


        [HttpGet]
        [Route("api/user/{userId}/items")]
        public async Task<HttpResponseMessage> Items(long userId, int page, int sizePerPage = 15)
        {
            var query = new GetUserWithFriendQuery(User.GetCloudentsUserId(), userId, page, sizePerPage);
            var result = await ZboxReadService.GetUserWithFriendItemsAsync(query);

            return Request.CreateResponse(result.Select(s => new
            {
                s.Id,
                s.BoxId,
                Source = s.Image,
                s.Name,
                s.Type
            }));

        }
    }
}
