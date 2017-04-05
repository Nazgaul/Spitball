using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.QnA;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries.QnA;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]

    public class QnAController : BaseController
    {
        private readonly IGuidIdGenerator m_IdGenerator;
        public QnAController(
            IGuidIdGenerator idGenerator)
        {
            m_IdGenerator = idGenerator;
        }

       

        [HttpGet, ZboxAuthorize(IsAuthenticationRequired = false), BoxPermission("id"), ActionName("Index")]
        public async Task<JsonResult> IndexAsync(long id, int top, int skip)
        {
            try
            {

                var query = new GetBoxQuestionsQuery(id, top, skip);
                var retVal =
                  await ZboxReadService.GetCommentsAsync(query).ConfigureAwait(false);
                //removing user name
                if (IsCrawler())
                {

                    return JsonOk(retVal.Select(s => new
                    {
                       
                        s.Content,
                        s.CreationTime,
                        s.Id,
                        s.RepliesCount,
                        s.LikesCount,
                        s.UserId,
                        s.Badges,
                        s.Score,
                        s.UserImage,
                        Files = s.Files.Select(x => new
                        {
                            x.Source,
                            x.Type,
                            x.Id,
                            x.Name
                        }),
                        Replies = s.Replies.Select(v =>
                            new
                            {
                                v.Content,
                                v.CreationTime,
                                v.Id,
                                v.UserId,
                                v.UserImage,

                                v.LikesCount,
                                Files = v.Files.Select(b => new
                                {
                                    b.Source,
                                    b.Type,
                                    b.Id,
                                    b.Name
                                })
                            })



                    }));
                }
                return JsonOk(retVal.Select(s => new
                {
                    s.Content,
                    s.CreationTime,
                    s.Id,
                    s.RepliesCount,
                    s.UserId,
                    s.Badges,
                    s.Score,
                    s.LikesCount,
                    s.UserImage,
                    s.UserName,
                    Files = s.Files.Select(x => new
                    {
                        x.Source,
                        x.Type,
                        x.Id,
                        x.Name
                    }),
                    Replies = s.Replies.Select(v =>
                    new
                    {
                        v.Content,
                        v.CreationTime,
                        v.Id,
                        v.UserId,
                        v.UserImage,
                        v.UserName,
                        s.Badges,
                        s.Score,
                        v.LikesCount,
                        Files = v.Files.Select(b => new
                        {
                            b.Source,
                            b.Type,
                            b.Id,
                            b.Name
                        })
                    }


                    )
                }));
            }
            catch (BoxAccessDeniedException)
            {
                return JsonError();
            }
            catch (BoxDoesntExistException)
            {
                return JsonError();
            }
        }

        [HttpGet, ZboxAuthorize(IsAuthenticationRequired = false), BoxPermission("id"), ActionName("Likes")]
        public async Task<JsonResult> LikesAsync(long id)
        {
            var query = new UserLikesQuery(User.GetUserId(), id);
            var retVal = await ZboxReadService.GetUserFeedLikesAsync(query);
            return JsonOk(retVal);
        }


        [ZboxAuthorize]
        [HttpPost]
        [RemoveBoxCookie, ActionName("AddComment")]
        public async Task<JsonResult> AddCommentAsync(Comment model)
        {
            if (string.IsNullOrEmpty(model.Content) && (model.Files == null || model.Files.Length == 0))
            {
                ModelState.AddModelError(string.Empty, BaseControllerResources.FillComment);
            }
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }

            var questionId = m_IdGenerator.GetId();
            var command = new AddCommentCommand(User.GetUserId(), model.BoxId, model.Content, questionId, model.Files, model.Anonymously);
            var details = await ZboxWriteService.AddCommentAsync(command).ConfigureAwait(false);
            return JsonOk(details);
        }

        [ZboxAuthorize]
        [HttpPost]
        [RemoveBoxCookie, ActionName("AddReply")]
        public async Task<JsonResult> AddReplyAsync(Reply model)
        {
            if (string.IsNullOrEmpty(model.Content) && (model.Files == null || model.Files.Length == 0))
            {
                ModelState.AddModelError(string.Empty, BaseControllerResources.FillComment);
            }
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var answerId = m_IdGenerator.GetId();
                var command = new AddReplyToCommentCommand(User.GetUserId(), model.BoxId, model.Content, answerId, model.CommentId, model.Files);
                await ZboxWriteService.AddReplyAsync(command);
                return JsonOk(answerId);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Add answer model: " + model, ex);
                return JsonError();
            }
        }


        [ZboxAuthorize]
        [HttpPost, ActionName("DeleteComment")]
        public async Task<JsonResult> DeleteCommentAsync(long boxId, Guid questionId)
        {
            try
            {
                var command = new DeleteCommentCommand(questionId, User.GetUserId(), boxId);
                await ZboxWriteService.DeleteCommentAsync(command);
                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"Delete question questionId {questionId} userid {User.GetUserId()}", ex);
                return JsonError();
            }
        }
        [ZboxAuthorize, ActionName("DeleteReply")]
        [HttpPost]
        public async Task<JsonResult> DeleteReplyAsync(long boxId, Guid answerId)
        {
            try
            {
                var command = new DeleteReplyCommand(answerId, User.GetUserId(), boxId);
                await ZboxWriteService.DeleteReplyAsync(command);
                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"Delete answer answerId {answerId} userid {User.GetUserId()}", ex);
                return JsonError();
            }
        }

        [ZboxAuthorize, HttpPost, ActionName("LikeComment")]
        public async Task<JsonResult> LikeCommentAsync(Guid commentId, long boxId)
        {
            var command = new LikeCommentCommand(commentId, User.GetUserId(), boxId);
            var retVal = await ZboxWriteService.LikeCommentAsync(command);
            return JsonOk(retVal.Liked);
        }

        [ZboxAuthorize, HttpPost, ActionName("LikeReply")]
        public async Task<JsonResult> LikeReplyAsync(Guid replyId, long boxId)
        {
            var command = new LikeReplyCommand(replyId, User.GetUserId(), boxId);
            var retVal = await ZboxWriteService.LikeReplyAsync(command);
            return JsonOk(retVal.Liked);
        }

        [HttpGet, BoxPermission("boxId"), ActionName("Replies")]
        public async Task<JsonResult> RepliesAsync(Guid id, Guid replyId, long boxId)
        {
            var query = new GetCommentRepliesQuery(boxId, id, replyId);
            var retVal =
                  await ZboxReadService.GetRepliesAsync(query);
            if (IsCrawler())
            {
                return JsonOk(retVal.Select(s => new
                {
                    s.Content,
                    s.CreationTime,
                    s.LikesCount,
                    Files = s.Files.Select(v => new
                    {
                        v.Id,
                        v.Source,
                        v.Type,
                        v.Name
                    }),
                    s.Id,
                    s.Badges,
                    s.Score,
                    s.UserId,
                    s.UserImage

                }));
            }
            return JsonOk(retVal.Select(s => new
            {
                s.Content,
                s.CreationTime,
                s.LikesCount,
                Files = s.Files.Select(v => new
                {
                    v.Id,
                    v.Source,
                    v.Type,
                    v.Name
                }),
                s.Id,
                s.Badges,
                s.Score,
                s.UserId,
                s.UserImage,
                s.UserName

            }));
        }





        [HttpGet, BoxPermission("boxId"), ZboxAuthorize, ActionName("CommentLikes")]
        public async Task<JsonResult> CommentLikesAsync(Guid id, long boxId)
        {
            var query = new GetFeedLikesQuery(id);
            var retVal = await ZboxReadService.GetCommentLikesAsync(query);
            return JsonOk(retVal);
        }

        [HttpGet, BoxPermission("boxId"), ZboxAuthorize, ActionName("ReplyLikes")]
        public async Task<JsonResult> ReplyLikesAsync(Guid id, long boxId)
        {
            var query = new GetFeedLikesQuery(id);
            var retVal = await ZboxReadService.GetReplyLikesAsync(query);
            return JsonOk(retVal);
        }

    }
}
