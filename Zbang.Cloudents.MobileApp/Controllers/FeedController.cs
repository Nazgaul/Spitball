using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.Qna;
using Zbang.Zbox.ViewModel.Queries.QnA;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    [Authorize]
    public class FeedController : ApiController
    {
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IQueueProvider m_QueueProvider;

        public FeedController(IZboxCacheReadService zboxReadService, IGuidIdGenerator guidGenerator, IZboxWriteService zboxWriteService, IQueueProvider queueProvider)
        {
            m_ZboxReadService = zboxReadService;
            m_GuidGenerator = guidGenerator;
            m_ZboxWriteService = zboxWriteService;
            m_QueueProvider = queueProvider;
        }


        /// <summary>
        /// created on 17/9/15 added quizzes to feed
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="page"></param>
        /// <param name="sizePerPage"></param>
        /// <returns></returns>
        [HttpGet]
        //, VersionedRoute("api/box/{boxId:long}/feed", 3)]
        [Route("api/box/{boxId:long}/feed")]
        public async Task<HttpResponseMessage> FeedAsync(long boxId, int page, int sizePerPage = 20)
        {
            try
            {
                var query = GetBoxQuestionsQuery.GetBoxQueryOldVersion(boxId, page, sizePerPage);
                var retVal =
                  await m_ZboxReadService.GetCommentsAsync(query);
                return Request.CreateResponse(retVal.Select(s => new
                {
                    s.Id,
                    Answers = LastReply(s),
                    
                    s.Content,
                    s.CreationTime,
                    Files = s.Files.Where(w=>w.Type != "flashcard").Select(v => new
                    {
                        v.Id,
                        v.Name,
                        v.OwnerId,
                        v.Type,
                        v.Source,
                        Thumbnail = string.IsNullOrEmpty(v.Source) ? null : "https://az779114.vo.msecnd.net/preview/" + HttpUtility.UrlPathEncode(v.Source) +
                              ".jpg?width=148&height=187&mode=crop"
                    }),
                    s.RepliesCount,
                    s.UserId,
                    s.UserImage,
                    s.UserName
                }));
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

        private static object LastReply(CommentDto s)
        {
            var lastReply = s.Replies.Select(x => new
            {
                x.Content,
                x.CreationTime,
                Files = x.Files.Where(w => w.Type != "flashcard").Select(z => new
                {
                    z.Id,
                    z.Name,
                    z.OwnerId,
                    z.Type,
                    z.Source,
                    Thumbnail = "https://az779114.vo.msecnd.net/preview/" + HttpUtility.UrlPathEncode(z.Source) +
                                ".jpg?width=148&height=187&mode=crop"
                }),
                x.Id,
                x.UserId,
                x.UserImage,
                x.UserName
            }).FirstOrDefault();
            if (lastReply == null)
            {
                return null;
            }
            return new[] {lastReply};
        }

        [HttpGet, Route("api/box/{boxId:long}/feed/{feedId:guid}")]
        public async Task<HttpResponseMessage> PostAsync(long boxId, Guid feedId)
        {
            var retVal =
                await m_ZboxReadService.GetCommentAsync(new GetQuestionQuery(feedId, boxId));

            return Request.CreateResponse(new
            {
                retVal.Files,
                retVal.Id,
                retVal.RepliesCount,
                retVal.UserId,
                retVal.UserImage,
                retVal.UserName,
                retVal.Content,
                retVal.CreationTime
            });
        }

        [HttpGet, Route("api/box/{boxId:long}/feed/{feedId:guid}/reply")]
        public async Task<HttpResponseMessage> GetRepliesAsync(long boxId, Guid feedId, /*Guid? belowReplyId,*/ int page, int sizePerPage = 20)
        {

            var replyId = /*belowReplyId ??*/ Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
            var retVal =
                 await m_ZboxReadService.GetRepliesAsync(new GetCommentRepliesQuery(boxId, feedId, replyId, page, sizePerPage));
            return Request.CreateResponse(retVal.Select(s => new
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
        // [VersionedRoute("api/box/{boxId:long}/feed", 2)]
        [Route("api/box/{boxId:long}/feed")]
        public async Task<HttpResponseMessage> PostCommentAnonymousAsync(long boxId, AddCommentRequest model)
        {
            if (string.IsNullOrEmpty(model.Content))
            {
                ModelState.AddModelError(string.Empty, "You need to write something or post files");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var questionId = m_GuidGenerator.GetId();
            var command = new AddCommentCommand(User.GetUserId(),
                boxId, model.Content, questionId, model.FileIds, model.Anonymously);
            var result = await m_ZboxWriteService.AddCommentAsync(command);
            return Request.CreateResponse(new
            {
                result.CommentId,
                result.UserId,
                result.UserImage,
                result.UserName
            });
        }

        [HttpPost]
        [Route("api/box/{boxId:long}/feed/{feedId:guid}/reply")]
        public async Task<HttpResponseMessage> PostReplyAsync(long boxId, Guid feedId, AddCommentRequest model)
        {
            if (string.IsNullOrEmpty(model.Content))
            {
                ModelState.AddModelError(string.Empty, "You need to write something or post files");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var answerId = m_GuidGenerator.GetId();
            var command = new AddReplyToCommentCommand(User.GetUserId(), boxId,
                model.Content, answerId, feedId, model.FileIds);
            await m_ZboxWriteService.AddReplyAsync(command);
            return Request.CreateResponse(answerId);
        }


        [HttpDelete]
        [Route("api/box/{boxId:long}/feed/{feedId:guid}")]
        public async Task<HttpResponseMessage> DeleteCommentAsync(long boxId, Guid feedId)
        {
            var command = new DeleteCommentCommand(feedId, User.GetUserId(), boxId);
            await m_ZboxWriteService.DeleteCommentAsync(command);
            return Request.CreateResponse();
        }

        [HttpDelete]
        [Route("api/box/{boxId:long}/reply/{replyId:guid}")]
        public async Task<HttpResponseMessage> DeleteReplyAsync(long boxId, Guid replyId)
        {
            var command = new DeleteReplyCommand(replyId, User.GetUserId(), boxId);
            await m_ZboxWriteService.DeleteReplyAsync(command);
            return Request.CreateResponse();
        }


        [Route("api/feed/flag")]
        [HttpPost]
        public async Task<HttpResponseMessage> FlagAsync(FlagPostReplyRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            await m_QueueProvider.InsertMessageToTranactionAsync(new BadPostData(User.GetUserId(), model.PostId));
            return Request.CreateResponse();
        }

    }
}
