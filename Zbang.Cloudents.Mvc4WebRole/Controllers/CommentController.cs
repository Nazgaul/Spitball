﻿using DevTrends.MvcDonutCaching;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Cloudents.Mvc4WebRole.Extensions;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class CommentController : BaseController
    {
        public CommentController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService,
            formsAuthenticationService)
        { }

        /// <summary>
        /// Ajax for Comment list.
        /// </summary>
        /// <param name="boxUid"></param>
        /// <returns></returns>
        [HttpGet]
        [Ajax]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult Index(long boxUid)
        {
           
            var userId = GetUserId(false);
            var query = new GetBoxCommentsQuery(boxUid, userId);
            var result = m_ZboxReadService.GetBoxComments(query);
            return this.CdJson(new JsonResponse(true, result));
        }


        ///// <summary>
        ///// Render the comment templates
        ///// </summary>
        ///// <returns></returns>
        //[DonutOutputCache(Duration = int.MaxValue, VaryByParam = "none", VaryByCustom = "Lang")] need to put mobile in here
        [ChildActionOnly]
        public ActionResult Comments()
        {
            return PartialView("_Comments");
        }

        /// <summary>
        /// Used in mobile
        /// </summary>
        /// <returns></returns>
        //[DonutOutputCache(Duration = int.MaxValue, VaryByParam = "none", VaryByCustom = "Lang")]
        [ChildActionOnly]
        public ActionResult Wall()
        {
            return PartialView();
        }








        //[HttpPost]
        //[ZboxAuthorize]
        //public ActionResult Reply(NewReply model)
        //{
        //    var userId = GetUserId();
        //    var boxid = m_ShortToLongCode.ShortCodeToLong(model.BoxUId);
        //    var command = new AddReplyToCommentCommand(userId, model.CommentToReplyId, model.CommentText, boxid);
        //    var result = m_ZboxWriteService.AddReplyToComment(command);

        //    var commentDto = new Zbox.ViewModel.DTOs.ActivityDtos.CommentDto(result.NewComment.Id, result.NewComment.Parent.Id,
        //        result.User.Name, result.User.Image, result.User.Id,
        //        DateTime.SpecifyKind(result.NewComment.DateTimeUser.CreationTime, DateTimeKind.Unspecified),
        //        null, null, null, null, result.NewComment.CommentText);

        //    return Json(new JsonResponse(true, commentDto));

        //}

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
                //var result = string.IsNullOrEmpty(newComment.ItemUid) ? AddBoxComment(newComment) : AddItemComment(newComment);
                //var result = AddBoxComment(newComment);
                return this.CdJson(new JsonResponse(true));
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError("Unauthorized", "You are not allowed to post comments");
                return this.CdJson(new JsonResponse(false, new { error = GetModelStateErrors() }));

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("AddComment user: {0} model: {1}", GetUserId(), newComment), ex);
                return this.CdJson(new JsonResponse(false, new { error = GetModelStateErrors() }));
            }

        }
        //private Zbox.ViewModel.DTOs.ActivityDtos.CommentDto AddBoxComment(NewComment comment)
        //{
        //    var command = new AddBoxCommentCommand(GetUserId(), comment.BoxUid, comment.CommentText);
        //    var result = m_ZboxWriteService.AddBoxComment(command);

        //    var commentDto = new Zbox.ViewModel.DTOs.ActivityDtos.CommentDto(
        //        result.NewComment.Id, null, result.User.Name, result.User.Image, result.User.Id,
        //        DateTime.SpecifyKind(result.NewComment.DateTimeUser.CreationTime, DateTimeKind.Unspecified),
        //        result.Target.Name, result.Target.Id.ToString(), null, null, result.NewComment.CommentText);

        //    return commentDto;

        //}


        #endregion

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult Delete(long commentId, long boxUid)
        {
            try
            {
                //var query = new DeleteCommentCommand(commentId, userId, boxUid);
                //m_ZboxWriteService.DeleteComment(query);
                return this.CdJson(new JsonResponse(true, commentId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeleteComment user: {0} boxid: {1} commentId {2}", GetUserId(), boxUid, commentId), ex);
                return this.CdJson(new JsonResponse(false));
            }
        }

        [HttpGet]
        [Ajax]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [AjaxCache(TimeToCache = TimeConsts.Minute * 5)]
        public async Task<ActionResult> Item(long itemId)
        {
            var userId = GetUserId(false);
            var query = new GetItemCommentsQuery(itemId, userId);
            var result = await m_ZboxReadService.GetItemComments(query);
            return this.CdJson(new JsonResponse(true, result));
        }

        [HttpPost]
        [ZboxAuthorize]
        [Ajax]
        public ActionResult AddAnnotation(NewAnnotation model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
            }
            try
            {
                var command = new AddAnnotationCommand(model.Comment, model.X, model.Y, model.Width, model.Height, model.ItemId, model.ImageId, GetUserId());
                m_ZboxWriteService.AddAnnotation(command);
                return this.CdJson(new JsonResponse(true, command.AnnotationId));
            }
            catch (UnauthorizedAccessException)
            {
                return this.CdJson(new JsonResponse(false));
            }
        }

        //[HttpPost]
        //[ZboxAuthorize]
        //[Ajax]
        //public ActionResult ReplyAnnotation(ReplyAnnotation model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
        //    }
        //    var command = new AddReplyToAnnotationCommand(GetUserId(), model.ItemId, model.ImageId, model.Comment, model.CommentId);
        //    m_ZboxWriteService.AddReplyAnnotation(command);
        //    return Json(new JsonResponse(true, command.ReplyId));
        //}

        [HttpPost]
        [ZboxAuthorize]
        [Ajax]
        public ActionResult DeleteAnnotation(DeleteAnnotation model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
            }
            var command = new DeleteAnnotationCommand(model.CommentId, GetUserId());
            m_ZboxWriteService.DeleteAnnotation(command);
            return this.CdJson(new JsonResponse(true));
        }
    }
}
