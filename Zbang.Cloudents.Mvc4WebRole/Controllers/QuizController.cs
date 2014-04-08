using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.Quiz;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Domain.Commands.Quiz;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]

    public class QuizController : BaseController
    {
        private readonly Lazy<IIdGenerator> m_IdGenerator;


        public QuizController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IFormsAuthenticationService formsAuthenticationService,
            Lazy<IIdGenerator> idGenerator)
            : base(zboxWriteService, zboxReadService,
            formsAuthenticationService)
        {
            m_IdGenerator = idGenerator;
        }
        //
        // GET: /Quiz/
        public ActionResult Index()
        {
            return View();
        }

        #region Quiz
        [HttpPost, Ajax]
        public ActionResult Create(Quiz model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var id = m_IdGenerator.Value.GetId(IdGenerator.QuizScope);
            var command = new CreateQuizCommand(GetUserId(), id, model.Name, model.BoxId);
            m_ZboxWriteService.CreateQuiz(command);

            return this.CdJson(new JsonResponse(true, id));
        }

        [HttpPost, Ajax]
        public ActionResult Update(UpdateQuiz model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var command = new UpdateQuizCommand(GetUserId(), model.Id, model.Text);
            m_ZboxWriteService.UpdateQuiz(command);
            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        public ActionResult Delete(long id)
        {
            var command = new DeleteQuizCommand(id, GetUserId());
            m_ZboxWriteService.DeleteQuiz(command);
            return this.CdJson(new JsonResponse(true));
        }

        [HttpPost, Ajax]
        public ActionResult Save(long id)
        {
            try
            {
                var command = new SaveQuizCommand(GetUserId(), id);
                m_ZboxWriteService.SaveQuiz(command);
                return this.CdJson(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                return this.CdJson(new JsonResponse(false, ex.Message));
            }
        }
        #endregion

        #region question
        [HttpPost, Ajax]
        public ActionResult CreateQuestion(Question model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            if (model.QuizId == 0)
            {
                ModelState.AddModelError(string.Empty,"Quiz id cannot be 0");
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));

            }
            var id = m_IdGenerator.Value.GetId();
            var command = new CreateQuestionCommand(model.Text, model.QuizId, GetUserId(), id);
            m_ZboxWriteService.CreateQuestion(command);
            return this.CdJson(new JsonResponse(true, id));
        }
        [HttpPost, Ajax]
        public ActionResult UpdateQuestion(UpdateQuestion model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var command = new UpdateQuestionCommand(GetUserId(), model.Id, model.Text);
            m_ZboxWriteService.UpdateQuestion(command);
            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        public ActionResult DeleteQuestion(Guid id)
        {
            var command = new DeleteQuestionCommand(GetUserId(), id);
            m_ZboxWriteService.DeleteQuestion(command);
            return this.CdJson(new JsonResponse(true));
        }
        #endregion

        #region Answer
        [HttpPost, Ajax]
        public ActionResult CreateAnswer(Answer model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var id = m_IdGenerator.Value.GetId();
            var command = new CreateAnswerCommand(GetUserId(), id, model.Text, model.CorrectAnswer, model.QuestionId);
            m_ZboxWriteService.CreateAnswer(command);
            return this.CdJson(new JsonResponse(true, id));
        }
        [HttpPost, Ajax]
        public ActionResult UpdateAnswer(UpdateAnswer model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }

            var command = new UpdateAnswerCommand(GetUserId(), model.Text, model.CorrectAnswer, model.Id);
            m_ZboxWriteService.UpdateAnswer(command);
            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        public ActionResult DeleteAnswer(Guid id)
        {
            var command = new DeleteAnswerCommand(GetUserId(), id);
            m_ZboxWriteService.DeleteAnswer(command);
            return this.CdJson(new JsonResponse(true));
        }
        #endregion
    }
}