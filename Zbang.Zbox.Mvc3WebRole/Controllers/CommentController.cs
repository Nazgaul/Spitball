using System;
using System.Web.Mvc;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.Mvc3WebRole.Helpers;
using Zbang.Zbox.Mvc3WebRole.Models;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class CommentController : BaseController
    {
        public CommentController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        { }
        
        /// <summary>
        /// Ajax for Comment list.
        /// </summary>
        /// <param name="BoxUid"></param>
        /// <returns></returns>
        [HttpPost]
        [NoCache] //ie loves to cache
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult Index(string BoxUid, string ItemUid)
        {
            if (string.IsNullOrEmpty(BoxUid))
            {
                return Json(new JsonResponse(false));
            }
            return string.IsNullOrEmpty(ItemUid) ? GetBoxComments(BoxUid) : GetItemComment(BoxUid, ItemUid);
        }
        ///// <summary>
        ///// Render the comment templates
        ///// </summary>
        ///// <returns></returns>
        ////[DonutOutputCache(Duration = int.MaxValue, VaryByParam = "none", VaryByCustom = "Lang")]
        //[ChildActionOnly]
        //public ActionResult Comments()
        //{
        //    return PartialView();
        //}

        ////[DonutOutputCache(Duration = int.MaxValue, VaryByParam = "none", VaryByCustom="Lang")]
        //[ChildActionOnly]
        //[AuthorizeVerifyEmailAttribute(Roles = ZboxRoleProvider.verifyEmailRole, IsAuthenticationRequired = false)]
        //public ActionResult CommentTemplate()
        //{
        //    return PartialView();
        //}


        [NonAction]
        private ActionResult GetBoxComments(string boxUid)
        {
            var boxId = m_ShortToLongCode.ShortCodeToLong(boxUid);
            var userId = GetUserId(false);

            var query = new GetBoxCommentsQuery(boxId, userId);
            var result = m_ZboxReadService.GetBoxComments(query);
            return Json(new JsonResponse(true, result));
        }

        /// <summary>
        /// Ajax for item Comment
        /// </summary>
        /// <param name="itemUid"></param>
        /// <param name="boxUid"></param>
        /// <returns></returns>     
        [NonAction]
        private ActionResult GetItemComment(string boxUid, string itemUid)
        {
            var itemId = m_ShortToLongCode.ShortCodeToLong(itemUid, ShortCodesType.Item);
            var boxId = m_ShortToLongCode.ShortCodeToLong(boxUid);
            var userId = GetUserId(false);
            var query = new GetItemCommentsQuery(itemId, userId, boxId);

            var result = m_ZboxReadService.GetItemComments(query);
            return Json(new JsonResponse(true, result));

        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult Reply(NewReply model)
        {
            var userId = GetUserId();
            var boxid = m_ShortToLongCode.ShortCodeToLong(model.BoxUId);
            var command = new AddReplyToCommentCommand(userId, model.CommentToReplyId, model.CommentText, boxid);
            var result = m_ZboxWriteService.AddReplyToComment(command);

            //var commentDto = new Zbox.ViewModel.DTOs.ActivityDtos.CommentDto(result.NewComment.Id, result.NewComment.Parent.Id,
            //    result.User.Name, result.User.Image, DateTime.SpecifyKind(result.NewComment.DateTimeUser.CreationTime, DateTimeKind.Unspecified),
            //    null, null, null, null, null, result.NewComment.CommentText);
                
                
            //var commentDto = new CommentDto
            //{
            //    AuthorName = result.User.Name,
            //    UserImage = result.User.Image,
            //    CreationTime = DateTime.SpecifyKind(result.NewComment.DateTimeUser.CreationTime, DateTimeKind.Unspecified),
            //    CommentText = result.NewComment.CommentText,
            //    Id = result.NewComment.Id,
            //    ParentId = result.NewComment.Parent.Id,
            //    BoxUid = model.BoxUId
            //};
            return Json(new JsonResponse(true, null));

        }

        #region AddComment
        
        [HttpPost]
        [ZboxAuthorize]
        public ActionResult Add(NewComment newComment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
                }
                var result = string.IsNullOrEmpty(newComment.ItemUid) ? AddBoxComment(newComment) : AddItemComment(newComment);
                return Json(new JsonResponse(true, result));
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError("Unauthorized", "You are not allowed to post comments");
                return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("AddComment user: {0} model: {1}", GetUserId(), newComment), ex);
                return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
            }

        }
        private Zbox.ViewModel.DTOs.ActivityDtos.CommentDto AddBoxComment(NewComment comment)
        {
            var boxid = m_ShortToLongCode.ShortCodeToLong(comment.BoxUid);
            var command = new AddBoxCommentCommand(GetUserId(), boxid, comment.CommentText);
            var result = m_ZboxWriteService.AddBoxComment(command);

            //var commentDto = new Zbox.ViewModel.DTOs.ActivityDtos.CommentDto(
            //    result.NewComment.Id,null,result.User.Name,result.User.Image,
            //    DateTime.SpecifyKind(result.NewComment.DateTimeUser.CreationTime, DateTimeKind.Unspecified),
            //    result.Target.Name,result.Target.Uid,null,null,null,result.NewComment.CommentText);
           
            return null;

        }

        private Zbox.ViewModel.DTOs.ActivityDtos.CommentDto AddItemComment(NewComment comment)
        {
            var boxid = m_ShortToLongCode.ShortCodeToLong(comment.BoxUid);
            var itemId = m_ShortToLongCode.ShortCodeToLong(comment.ItemUid, ShortCodesType.Item);
            var command = new AddItemCommentCommand(GetUserId(), itemId, boxid, comment.CommentText);
            var result = m_ZboxWriteService.AddItemComment(command);

            //var commentDto = new Zbox.ViewModel.DTOs.ActivityDtos.CommentDto(
            //    result.ItemComment.Id,null,result.User.Name,result.User.Image,
            //    DateTime.SpecifyKind(result.ItemComment.DateTimeUser.CreationTime, DateTimeKind.Unspecified),
            //    null, null, null, null, null, result.ItemComment.CommentText);

            return null;
        }
        #endregion

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult Delete(long CommentId, string BoxUid)
        {
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
                var userId = GetUserId();
                var query = new DeleteCommentCommand(CommentId, userId, boxId);
                m_ZboxWriteService.DeleteComment(query);
                return Json(new JsonResponse(true, CommentId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeleteComment user: {0} boxid: {1} commentId {2}", GetUserId(), BoxUid, CommentId), ex);
                return Json(new JsonResponse(false));
            }
        }
    }
}
