using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.MvcWebRole.Helpers;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.MvcWebRole.Controllers
{
    public class CommentController : Controller
    {
        private IZboxService m_ZboxService;

        public CommentController(IZboxService zboxService)
        {
            
            m_ZboxService = zboxService;
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddBoxComment(string commentText, long targetId)
        {
            JsonResponse response = null;

            try
            {

                AddBoxCommentCommand command = new AddBoxCommentCommand(ExtractUserID.GetUserEmailId(), targetId, commentText);
                AddBoxCommentCommandResult result = m_ZboxService.AddBoxComment(command);

                CommentDto commentDto = new CommentDto()
                {
                    CommentId = result.NewComment.Id,
                    AuthorName = this.User.Identity.Name,
                    CommentText = result.NewComment.CommentText,
                    CreationTime = result.NewComment.DateTimeUser.CreationTime,
                    IsUserDeleteAllowed = true, // auther can always delete his comments

                };

                response = new JsonResponse(true, new { comment = commentDto, boxId = targetId });
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("AddBoxComment userId: {0} commentText: {1} targetId {2}", this.User.Identity.Name, commentText, targetId));
                TraceLog.WriteError(ex);

                response = new JsonResponse(false, "problem with adding a comment");
            }
            return this.Json(response);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddItemComment(string commentText, long itemId, long boxId)
        {
            JsonResponse response = null;

            try
            {

                AddItemCommentCommand command = new AddItemCommentCommand(ExtractUserID.GetUserEmailId(), itemId, boxId, commentText);
                AddItemCommentCommandResult result = m_ZboxService.AddItemComment(command);

                CommentDto commentDto = new CommentDto()
                {
                    CommentId = result.ItemComment.Id,
                    AuthorName = this.User.Identity.Name,
                    CommentText = result.ItemComment.CommentText,
                    CreationTime = result.ItemComment.DateTimeUser.CreationTime,
                    IsUserDeleteAllowed = true, // auther can always delete his comments

                };

                response = new JsonResponse(true, new { comment = commentDto, itemId = itemId });
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("AddItemComment userId: {0} commentText: {1} itemId {2} boxid {3}", this.User.Identity.Name, commentText, itemId, boxId));
                TraceLog.WriteError(ex);

                response = new JsonResponse(false, "problem with adding item comment");
            }
            return this.Json(response);
        }

        /// <summary>
        /// Add a reply to a comment - only apply to box comment
        /// </summary>
        /// <param name="commentText"></param>
        /// <param name="targetId"></param>
        /// <param name="boxId"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddReply(string commentText, long targetId, long boxId)
        {
            JsonResponse response = null;

            try
            {
                AddReplyCommand command = new AddReplyCommand(ExtractUserID.GetUserEmailId(), targetId, commentText, boxId);
                AddReplyCommandResult result = m_ZboxService.AddReply(command);

                // avoid circular reference problem by sending a dto
                CommentDto dto = new CommentDto()
                {
                    CommentId = result.NewComment.Id,
                    AuthorName = this.User.Identity.Name,
                    CommentText = result.NewComment.CommentText,
                    CreationTime = result.NewComment.DateTimeUser.CreationTime,
                 //   ParentId = targetId,
                    IsUserDeleteAllowed = true // auther can always delete his comments
                };

                response = new JsonResponse(true, new { comment = dto, boxId = boxId });
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("comment text: {0},target id: {1}, user {2}", commentText, targetId, this.User.Identity.Name));
                TraceLog.WriteError(ex);
                response = new JsonResponse(false, "Problem with add a reply");
            }
            return this.Json(response);
        }



        /// <summary>
        /// Get box comment - used in the unreg page
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        [CompressFilter]
        [AcceptVerbs(HttpVerbs.Get)]        
        public JsonResult GetBoxComments(string boxId)
        {
            var BoxId = ShortCodesCache.ShortCodeToLong(boxId);
            GetBoxCommentsQuery query = new GetBoxCommentsQuery(BoxId, ExtractUserID.GetUserEmailId(false));
            JsonResponse response = null;
            try
            {
                IEnumerable<CommentDto> commentDtos = m_ZboxService.GetBoxComments(query);

                response = new JsonResponse(true, new { boxId = boxId, comments = commentDtos });
            }
            catch (BoxAccessDeniedException ex)
            {
                response = new JsonResponse(false, "You do not have permission to view this comments");
                TraceLog.WriteInfo("Get Box Comments boxid " + boxId);
                TraceLog.WriteError(ex);
            }
            catch (Exception ex)
            {
                response = new JsonResponse(false, "Error getting box comment");
                TraceLog.WriteInfo("Get Box Comments boxid " + boxId);
                TraceLog.WriteError(ex);
            }
           

            return this.Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemComments(long boxId, long itemId)
        {
            GetItemCommentsQuery query = new GetItemCommentsQuery(itemId, ExtractUserID.GetUserEmailId(false), boxId);
            JsonResponse response = null;
            try
            {
                IEnumerable<CommentDto> commentDtos = m_ZboxService.GetItemComments(query);

                response = new JsonResponse(true, new { boxId = boxId, itemId = itemId, comments = commentDtos });
            }
            catch (BoxAccessDeniedException ex)
            {
                response = new JsonResponse(false, "You do not have permission to view this comments");
                TraceLog.WriteInfo(string.Format("GetItemComments boxid {0} itemid {1}", boxId, itemId));
                TraceLog.WriteError(ex);
            }
            catch (Exception ex)
            {
                response = new JsonResponse(false, "Error getting box comment");
                TraceLog.WriteInfo(string.Format("GetItemComments boxid {0} itemid {1}", boxId, itemId));
                TraceLog.WriteError(ex);
            }

            return this.Json(response, JsonRequestBehavior.AllowGet);
        }      

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteComment(long commentId, long boxId)
        {
            var userEmail = ExtractUserID.GetUserEmailId();
            try
            {
                DeleteCommentCommand command = new DeleteCommentCommand(commentId, userEmail, boxId);
                DeleteCommentCommandResult result = m_ZboxService.DeleteComment(command);

                return this.Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("DeleteComment commentId {0} userId {1} ", commentId, userEmail));
                TraceLog.WriteError(ex);
                return this.Json(new JsonResponse(false, "problem with deleting a comment"));
            }
        }

    }


}
