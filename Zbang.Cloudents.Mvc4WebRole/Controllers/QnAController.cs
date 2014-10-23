using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models.QnA;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]

    public class QnAController : BaseController
    {
        private readonly Lazy<IIdGenerator> m_IdGenerator;
        public QnAController(
            Lazy<IIdGenerator> idGenerator)
        {
            m_IdGenerator = idGenerator;
        }


        [ZboxAuthorize]
        [Ajax, HttpPost]
        public JsonResult AddQuestion(Question model)
        {
            if (string.IsNullOrEmpty(model.Content) && (model.Files==null || model.Files.Length == 0))
            {
                ModelState.AddModelError(string.Empty, "You need to write something or post files");
            }
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }

            var questionId = m_IdGenerator.Value.GetId();
            var command = new AddCommentCommand(GetUserId(), model.BoxId, model.Content, questionId, model.Files);
            ZboxWriteService.AddQuestion(command);
            return Json(new JsonResponse(true, questionId));
        }

        [ZboxAuthorize]
        [Ajax, HttpPost]
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
                var answerId = m_IdGenerator.Value.GetId();
                var command = new AddAnswerToQuestionCommand(GetUserId(), model.BoxId, model.Content, answerId, model.QuestionId, model.Files);
                await ZboxWriteService.AddAnswer(command);
                return Json(new JsonResponse(true, answerId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Add answer model: " + model, ex);
                return Json(new JsonResponse(false));
            }
        }
       

        [ZboxAuthorize]
        [Ajax, HttpPost]
        public JsonResult RemoveFile(long itemId)
        {
            var command = new DeleteFileFromQnACommand(itemId, GetUserId());
            ZboxWriteService.DeleteFileFromQnA(command);
            return Json(new JsonResponse(true));
        }

        [ZboxAuthorize]
        [Ajax, HttpPost]
        public JsonResult DeleteQuestion(Guid questionId)
        {
            try
            {
                var command = new DeleteCommentCommand(questionId, GetUserId());
                ZboxWriteService.DeleteComment(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Delete question questionId {0} userid {1}", questionId.ToString(), GetUserId()), ex);
                return Json(new JsonResponse(false));
            }
        }
        [ZboxAuthorize]
        [Ajax, HttpPost]
        public JsonResult DeleteAnswer(Guid answerId)
        {
            try
            {
                var command = new DeleteReplyCommand(answerId, GetUserId());
                ZboxWriteService.DeleteAnswer(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Delete answer answerId {0} userid {1}", answerId, GetUserId()), ex);
                return Json(new JsonResponse(false));
            }
        }


        [Ajax, HttpGet, ZboxAuthorize(IsAuthenticationRequired = false)]
        public async Task<JsonResult> Index(long boxId)
        {
            try
            {
                var retVal =
                  await  ZboxReadService.GetQuestions(new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(boxId,
                        GetUserId(false)));


                retVal.ToList().ForEach(f =>
                {
                    f.Files.ForEach(fi => fi.DownloadUrl = Url.RouteUrl("ItemDownload2", new {boxId, itemId = fi.Id}));
                    f.Answers.ForEach(
                        fa =>
                            fa.Files.ForEach(
                                fi1 => fi1.DownloadUrl = Url.RouteUrl("ItemDownload2", new {boxId, itemId = fi1.Id})));
                });
                return Json(new JsonResponse(true, retVal));
            }
            catch (BoxAccessDeniedException)
            {
                return Json(new JsonResponse(false));
            }
            catch (BoxDoesntExistException)
            {
                return Json(new JsonResponse(false));
            }
        }



    }
}
