﻿using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
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
        [HttpPost]
        [RemoveBoxCookie]
        public async Task<JsonResult> AddQuestion(Question model)
        {
            if (string.IsNullOrEmpty(model.Content) && (model.Files == null || model.Files.Length == 0))
            {
                ModelState.AddModelError(string.Empty, "You need to write something or post files");
            }
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorsFromModelState());
            }

            var questionId = m_IdGenerator.Value.GetId();
            var command = new AddCommentCommand(User.GetUserId(), model.BoxId, model.Content, questionId, model.Files);
            await ZboxWriteService.AddQuestionAsync(command);
            return JsonOk(questionId);
        }

        [ZboxAuthorize]
        [HttpPost]
        [RemoveBoxCookie]
        public async Task<JsonResult> AddAnswer(Answer model)
        {
            if (string.IsNullOrEmpty(model.Content) && (model.Files == null || model.Files.Length == 0))
            {
                ModelState.AddModelError(string.Empty, "You need to write something or post files");
            }
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorsFromModelState());
            }
            try
            {
                var answerId = m_IdGenerator.Value.GetId();
                var command = new AddAnswerToQuestionCommand(User.GetUserId(), model.BoxId, model.Content, answerId, model.QuestionId, model.Files);
                await ZboxWriteService.AddAnswerAsync(command);
                return JsonOk(answerId);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Add answer model: " + model, ex);
                return JsonError();
            }
        }


        [ZboxAuthorize]
        [HttpPost]
        public JsonResult DeleteQuestion(Guid questionId)
        {
            try
            {
                var command = new DeleteCommentCommand(questionId, User.GetUserId());
                ZboxWriteService.DeleteComment(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Delete question questionId {0} userid {1}", questionId.ToString(), User.GetUserId()), ex);
                return JsonError();
            }
        }
        [ZboxAuthorize]
        [HttpPost]
        public JsonResult DeleteAnswer(Guid answerId)
        {
            try
            {
                var command = new DeleteReplyCommand(answerId, User.GetUserId());
                ZboxWriteService.DeleteAnswer(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Delete answer answerId {0} userid {1}", answerId, User.GetUserId()), ex);
                return JsonError();
            }
        }


        [HttpGet, ZboxAuthorize(IsAuthenticationRequired = false), BoxPermission("boxId")]
        public async Task<JsonResult> Index(long boxId)
        {
            try
            {
                var retVal =
                  await ZboxReadService.GetQuestions(new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(boxId));
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
