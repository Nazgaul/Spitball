using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Cloudents.MobileApp2.Models;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
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

        public IPushNotification PushNotification { get; set; }

        // GET api/Feed
        [HttpGet]
        [Route("api/box/{boxId:long}/feed")]
        public async Task<HttpResponseMessage> Feed([FromUri]long boxId, int page)
        {
            try
            {
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
        public async Task<HttpResponseMessage> PostComment([FromUri] long boxId, [FromBody] AddCommentRequest model)
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
            var t1 = ZboxWriteService.AddQuestionAsync(command);
            var t2 = PushNotification.SendPush(boxId);
            await Task.WhenAll(t1, t2);
            return Request.CreateResponse(questionId);
        }

        [HttpPost]
        [Route("api/box/{boxId:long}/feed/{feedId:guid}/reply")]
        public async Task<HttpResponseMessage> PostReply([FromUri] long boxId, [FromUri] Guid feedId, [FromBody] AddCommentRequest model)
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
            var t1 = ZboxWriteService.AddAnswerAsync(command);
            var t2 = PushNotification.SendPush(boxId); 
            await Task.WhenAll(t1, t2);
            return Request.CreateResponse(answerId);
        }


        [HttpDelete]
        [Route("api/box/{boxId:long}/feed/{feedId:guid}")]
        public HttpResponseMessage DeleteComment([FromUri] long boxId, [FromUri] Guid feedId)
        {
            var command = new DeleteCommentCommand(feedId, User.GetCloudentsUserId());
            ZboxWriteService.DeleteComment(command);
           return Request.CreateResponse();
        }

        [HttpDelete]
        [Route("api/box/{boxId:long}/reply/{replyId:guid}")]
        public HttpResponseMessage DeleteReply([FromUri] long boxId, [FromUri] Guid replyId)
        {
            var command = new DeleteReplyCommand(replyId, User.GetCloudentsUserId());
            ZboxWriteService.DeleteAnswer(command);
            return Request.CreateResponse();
        }

    }
}
