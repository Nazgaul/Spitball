using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Cloudents.MobileApp2.Models;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Domain.Commands;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class FeedController : ApiController
    {
        public ApiServices Services { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }

        public IGuidIdGenerator GuidGenerator { get; set; }

        public IZboxWriteService ZboxWriteService { get; set; }
        public IQueueProvider QueueProvider { get; set; }


        // GET api/Feed
        [HttpGet]
        [VersionedRoute("api/box/{boxId:long}/feed", 1)]
        //[Route("{boxId:long}/feed")]
        public async Task<HttpResponseMessage> Feed(long boxId, int page, int sizePerPage = 20)
        {
            try
            {
                var retVal =
                  await ZboxReadService.GetQuestionsWithAnswers(new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(boxId, page, sizePerPage));
                return Request.CreateResponse(retVal);
            }
            catch (BoxAccessDeniedException)
            {
                return Request.CreateUnauthorizedResponse();
            }
            catch (BoxDoesntExistException)
            {
                return Request.CreateNotFoundResponse();
            }
        }

        [HttpGet, VersionedRoute("api/box/{boxId:long}/feed", 2)]
        public async Task<HttpResponseMessage> Feed2(long boxId, int page, int sizePerPage = 20)
        {
            try
            {
                var retVal =
                  await ZboxReadService.GetQuestionsWithLastAnswer(new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(boxId, page, sizePerPage));
                return Request.CreateResponse(retVal);
            }
            catch (BoxAccessDeniedException)
            {
                return Request.CreateUnauthorizedResponse();
            }
            catch (BoxDoesntExistException)
            {
                return Request.CreateNotFoundResponse();
            }
        }

        [HttpGet, Route("api/box/{boxId:long}/feed/{feedId:guid}/reply")]
        public async Task<HttpResponseMessage> GetReplies(long boxId, Guid feedId, int page, int sizePerPage = 20)
        {
            var retVal =
                 await ZboxReadService.GetReplies(new Zbox.ViewModel.Queries.QnA.GetCommentRepliesQuery(boxId,feedId,page,sizePerPage));
            return Request.CreateResponse(retVal.Select(s=> new
            {
                s.Id,
                s.UserImage,
                s.UserName,
                s.UserId,
                s.Content,
                s.CreationTime,

                Files = s.Files.Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.OwnerId,
                    d.Type,
                    d.Source,
                    Thumbnail = "https://az779114.vo.msecnd.net/preview/" + HttpUtility.UrlPathEncode(d.Source) +
                               ".jpg?width=148&height=187&mode=crop"
                })
                
                
            }));
        }


        [HttpPost]
        [Route("api/box/{boxId:long}/feed")]
        public async Task<HttpResponseMessage> PostComment(long boxId, AddCommentRequest model)
        {
            if (string.IsNullOrEmpty(model.Content))
            {
                ModelState.AddModelError(string.Empty, "You need to write something or post files");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var questionId = GuidGenerator.GetId();
            var command = new AddCommentCommand(User.GetCloudentsUserId(),
                boxId, model.Content, questionId, model.FileIds);
            await ZboxWriteService.AddQuestionAsync(command);
            return Request.CreateResponse(questionId);
        }

        [HttpPost]
        [Route("api/box/{boxId:long}/feed/{feedId:guid}/reply")]
        public async Task<HttpResponseMessage> PostReply(long boxId, Guid feedId, AddCommentRequest model)
        {
            if (string.IsNullOrEmpty(model.Content))
            {
                ModelState.AddModelError(string.Empty, "You need to write something or post files");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var answerId = GuidGenerator.GetId();
            var command = new AddAnswerToQuestionCommand(User.GetCloudentsUserId(), boxId,
                model.Content, answerId, feedId, model.FileIds);
            await ZboxWriteService.AddAnswerAsync(command);
            return Request.CreateResponse(answerId);
        }


        [HttpDelete]
        [Route("api/box/{boxId:long}/feed/{feedId:guid}")]
        public HttpResponseMessage DeleteComment(long boxId, Guid feedId)
        {
            var command = new DeleteCommentCommand(feedId, User.GetCloudentsUserId());
            ZboxWriteService.DeleteComment(command);
            return Request.CreateResponse();
        }

        [HttpDelete]
        [Route("api/box/{boxId:long}/reply/{replyId:guid}")]
        public HttpResponseMessage DeleteReply(long boxId, Guid replyId)
        {
            var command = new DeleteReplyCommand(replyId, User.GetCloudentsUserId());
            ZboxWriteService.DeleteAnswer(command);
            return Request.CreateResponse();
        }


        [Route("api/feed/flag")]
        [HttpPost]
        public async Task<HttpResponseMessage> Flag(FlagPostReplyRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            await QueueProvider.InsertMessageToTranactionAsync(new BadPostData(User.GetCloudentsUserId(), model.PostId));
            return Request.CreateResponse();
        }

    }
}
