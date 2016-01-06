using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
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
        private readonly IGuidIdGenerator m_IdGenerator;
        public QnAController(
            IGuidIdGenerator idGenerator)
        {
            m_IdGenerator = idGenerator;
        }


        [ZboxAuthorize]
        [HttpPost]
        [RemoveBoxCookie]
        public async Task<JsonResult> AddComment(Comment model)
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
            var details = await ZboxWriteService.AddQuestionAsync(command);
            return JsonOk(details);
        }

        [ZboxAuthorize]
        [HttpPost]
        [RemoveBoxCookie]
        public async Task<JsonResult> AddReply(Reply model)
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
                var command = new AddAnswerToQuestionCommand(User.GetUserId(), model.BoxId, model.Content, answerId, model.CommentId, model.Files);
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
                return JsonOk();
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
                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Delete answer answerId {0} userid {1}", answerId, User.GetUserId()), ex);
                return JsonError();
            }
        }


        [HttpGet, ZboxAuthorize(IsAuthenticationRequired = false), BoxPermission("id")]
        public async Task<JsonResult> Index(long id, int page)
        {
            try
            {
                var query = new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(id, page, 20);
                var retVal =
                  await ZboxReadService.GetQuestionsWithAnswersAsync(query);
                //removing user name
                if (Request.Browser.Crawler)
                {

                    return JsonOk(retVal.Select(s => new
                    {
                        s.Content,
                        s.CreationTime,
                        s.Id,
                        s.Url,
                        s.UserId,
                        s.UserImage,
                        s.Files,
                        Answers = s.Answers.Select(v => new
                        {
                            v.Content,
                            v.CreationTime,
                            v.Id,
                            v.Url,
                            v.UserId,
                            v.UserImage,
                            v.Files
                        })

                    }));
                }
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
