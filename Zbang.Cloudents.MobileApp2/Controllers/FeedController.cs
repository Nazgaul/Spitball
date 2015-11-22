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
                  await ZboxReadService.GetQuestionsWithAnswersAsync(new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(boxId, page, sizePerPage));
                return Request.CreateResponse(retVal.Select(s => new
                {
                    Answers = s.Answers.Select(a => new
                    {
                        a.Content,
                        a.CreationTime,
                        Files = a.Files.Select(v => new
                        {
                            v.AnswerId,
                            v.Id,
                            v.Name,
                            v.OwnerId,
                            v.QuestionId,
                            v.Source,
                            Thumbnail = "https://az779114.vo.msecnd.net/preview/" + HttpUtility.UrlPathEncode(v.Source) +
                                      ".jpg?width=148&height=187&mode=crop",
                            v.Type,
                            v.Url,
                            v.DownloadUrl
                        }),
                        a.Id,
                        a.QuestionId,
                        a.Url,
                        a.UserId,
                        a.UserImage,
                        a.UserName
                    }),
                    s.Content,
                    s.CreationTime,
                    Files = s.Files.Select(v => new
                    {
                        v.AnswerId,
                        v.Id,
                        v.Name,
                        v.OwnerId,
                        v.QuestionId,
                        v.Source,
                        Thumbnail = "https://az779114.vo.msecnd.net/preview/" + HttpUtility.UrlPathEncode(v.Source) +
                                  ".jpg?width=148&height=187&mode=crop",
                        v.Type,
                        v.Url,
                        v.DownloadUrl
                    }),
                    s.Id,
                    s.Url,
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

        [HttpGet, VersionedRoute("api/box/{boxId:long}/feed", 2)]
        public async Task<HttpResponseMessage> Feed2(long boxId, int page, int sizePerPage = 20)
        {
            try
            {
                var retVal =
                  await ZboxReadService.GetQuestionsWithLastAnswerAsync(new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(boxId, page, sizePerPage));
                return Request.CreateResponse(retVal.Select(s => new
                {
                    s.Id,
                    Answers = s.Answers.Select(x => new
                    {
                        x.Content,
                        x.CreationTime,
                        Files = x.Files.Select(z => new
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
                    }),
                    s.Content,
                    s.CreationTime,
                    Files = s.Files.Where(w=> w.Source != null ).Select(v => new
                    {
                        v.Id,
                        v.Name,
                        v.OwnerId,
                        v.Type,
                        v.Source,
                        Thumbnail = "https://az779114.vo.msecnd.net/preview/" + HttpUtility.UrlPathEncode(v.Source) +
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

        /// <summary>
        /// created on 17/9/15 added quizzes to feed
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="page"></param>
        /// <param name="sizePerPage"></param>
        /// <returns></returns>
        [HttpGet, VersionedRoute("api/box/{boxId:long}/feed", 3)]
        public async Task<HttpResponseMessage> Feed3(long boxId, int page, int sizePerPage = 20)
        {
            try
            {
                var retVal =
                  await ZboxReadService.GetQuestionsWithLastAnswerAsync(new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(boxId, page, sizePerPage));
                return Request.CreateResponse(retVal.Select(s => new
                {
                    s.Id,
                    Answers = s.Answers.Select(x => new
                    {
                        x.Content,
                        x.CreationTime,
                        Files = x.Files.Select(z => new
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
                    }),
                    s.Content,
                    s.CreationTime,
                    Files = s.Files.Select(v => new
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

        [HttpGet, Route("api/box/{boxId:long}/feed/{feedId:guid}")]
        public async Task<HttpResponseMessage> Post(long boxId, Guid feedId)
        {
            var retVal =
                await ZboxReadService.GetQuestionAsync(new Zbox.ViewModel.Queries.QnA.GetQuestionQuery(feedId, boxId));

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
        public async Task<HttpResponseMessage> GetReplies(long boxId, Guid feedId, int page, int sizePerPage = 20)
        {
            var retVal =
                 await ZboxReadService.GetRepliesAsync(new Zbox.ViewModel.Queries.QnA.GetCommentRepliesQuery(boxId, feedId, page, sizePerPage));
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
        [VersionedRoute("api/box/{boxId:long}/feed", 1)]
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
                boxId, model.Content, questionId, model.FileIds, model.Anonymously);
            await ZboxWriteService.AddQuestionAsync(command);
            return Request.CreateResponse(questionId);
        }

        [HttpPost]
        [VersionedRoute("api/box/{boxId:long}/feed", 2)]
        public async Task<HttpResponseMessage> PostCommentAnonymous(long boxId, AddCommentRequest model)
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
                boxId, model.Content, questionId, model.FileIds, model.Anonymously);
            var result = await ZboxWriteService.AddQuestionAsync(command);
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
