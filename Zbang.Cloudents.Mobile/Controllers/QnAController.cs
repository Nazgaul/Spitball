using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mobile.Helpers;
using Zbang.Cloudents.Mobile.Models.QnA;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]

    public class QnAController : BaseController
    {
        private readonly IGuidIdGenerator m_GuidGenerator;
        public QnAController(
            IGuidIdGenerator guidGenerator)
        {
            m_GuidGenerator = guidGenerator;
        }

        [ZboxAuthorize]
        [HttpPost]
        public async Task<JsonResult> AddQuestion(Question model)
        {
            if (string.IsNullOrEmpty(model.Content) && (model.Files == null || model.Files.Length == 0))
            {
                ModelState.AddModelError(string.Empty, "You need to write something or post files");
            }
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }

            var questionId = m_GuidGenerator.GetId();
            var command = new AddCommentCommand(User.GetUserId(), model.BoxId, model.Content, questionId, model.Files, false);
            await ZboxWriteService.AddQuestionAsync(command);
            return Json(new JsonResponse(true, questionId));
        }

        [ZboxAuthorize]
        [HttpPost]
        public async Task<JsonResult> AddAnswer(Answer model)
        {
            if (string.IsNullOrEmpty(model.Content) && (model.Files == null || model.Files.Length == 0))
            {
                ModelState.AddModelError(string.Empty, "You need to write something or post files");
            }
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            try
            {
                var answerId = m_GuidGenerator.GetId();
                var command = new AddAnswerToQuestionCommand(User.GetUserId(), model.BoxId, model.Content, answerId, model.QuestionId, model.Files);
                await ZboxWriteService.AddAnswerAsync(command);
                return Json(new JsonResponse(true, answerId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Add answer model: " + model, ex);
                return Json(new JsonResponse(false));
            }
        }


        //[ZboxAuthorize]
        //[HttpPost]
        //public JsonResult DeleteQuestion(Guid questionId)
        //{
        //    try
        //    {
        //        var command = new DeleteCommentCommand(questionId, User.GetUserId());
        //        ZboxWriteService.DeleteComment(command);
        //        return Json(new JsonResponse(true));
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError(string.Format("Delete question questionId {0} userid {1}", questionId.ToString(), User.GetUserId()), ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}
        //[ZboxAuthorize]
        //[HttpPost]
        //public JsonResult DeleteAnswer(Guid answerId)
        //{
        //    try
        //    {
        //        var command = new DeleteReplyCommand(answerId, User.GetUserId());
        //        ZboxWriteService.DeleteAnswer(command);
        //        return Json(new JsonResponse(true));
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError(string.Format("Delete answer answerId {0} userid {1}", answerId, User.GetUserId()), ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}


        [HttpGet, ZboxAuthorize(IsAuthenticationRequired = false),
        BoxPermission("boxId")]
        public async Task<JsonResult> Index(long boxId, int page)
        {
            try
            {
                var retVal =
                  await ZboxReadService.GetQuestionsWithAnswers(new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(boxId, page, 20));
                return JsonOk(retVal);
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



    }
}
