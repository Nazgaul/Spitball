using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Notifications;
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


        // GET api/Feed
        [HttpGet]
        [Route("api/box/{boxId:long}/feed")]
        public async Task<HttpResponseMessage> Feed(long boxId, int page)
        {
            try
            {
                //TODO: check box permission
                var retVal =
                  await ZboxReadService.GetQuestions(new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(boxId, page, 20));
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
                boxId, model.Content, questionId, null);
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
                model.Content, answerId, feedId, null);
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

    }
}
