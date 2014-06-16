using System;
using System.Linq;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.Quiz;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.ViewModel.Queries;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]

    [CompressFilter]
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
        [Route("Quiz/{universityName}/{boxId:long}/{boxName}/{quizId:long:min(0)}/{quizName}", Name = "Quiz")]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [NonAjax]
        public async Task<ActionResult> Index(long boxId, long quizId, string quizName, string universityName, string boxName)
        {

            var model = await GetQuiz(boxId, quizId, quizName, true);

            var serializer = new JsonNetSerializer();
            ViewBag.userD = serializer.Serialize(model.Sheet);

            var builder = new UrlBuilder(HttpContext);
            var url = builder.BuildBoxUrl(model.Quiz.BoxId, boxName, universityName);

            ViewBag.boxName = boxName;
            ViewBag.boxUrl = url;


            if (model.Quiz.Seo != null && !string.IsNullOrEmpty(model.Quiz.Seo.Country))
            {
                var culture = Languages.GetCultureBaseOnCountry(model.Quiz.Seo.Country);
                BaseControllerResources.Culture = culture;
                ViewBag.title = string.Format("{0} {1} | {2} {3} | {4} | {5}", BaseControllerResources.QuizTitlePrefix, model.Quiz.Name, BaseControllerResources.QuizTitleText, model.Quiz.Seo.BoxName, model.Quiz.Seo.UniversityName, BaseControllerResources.Cloudents);
            }
            ViewBag.metaDescription = model.Quiz.Questions.First().Text;

            return View(model.Quiz);
        }
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [Ajax]
        [ActionName("Index")]
        public async Task<ActionResult> IndexAjax(long boxId, long quizId, string quizName, string universityName, string boxName)
        {
            var model = await GetQuiz(boxId, quizId, quizName, false);
            var serializer = new JsonNetSerializer();
            ViewBag.userD = serializer.Serialize(model.Sheet);

            var builder = new UrlBuilder(HttpContext);
            var url = builder.BuildBoxUrl(model.Quiz.BoxId, boxName, universityName);

            ViewBag.boxName = boxName;
            ViewBag.boxUrl = url;


            return PartialView(model.Quiz);

        }

        [Ajax]
        [ZboxAuthorize]
        public async Task<ActionResult> Discussion(long quizId)
        {
            var query = new GetDisscussionQuery(quizId);
            var model = await m_ZboxReadService.GetDiscussion(query);
            return this.CdJson(new JsonResponse(true, model));
        }

        [NonAction]
        private async Task<Zbox.ViewModel.DTOs.ItemDtos.QuizWithDetailSolvedDto> GetQuiz(long boxId, long quizId, string quizName, bool isNonAjax)
        {
            var query = new GetQuizQuery(quizId, GetUserId(false), boxId, isNonAjax);
            var model = await m_ZboxReadService.GetQuiz(query);
            if (model.Quiz.BoxId != boxId)
            {
                throw new ItemNotFoundException();
            }
            if (quizName != UrlBuilder.NameToQueryString(model.Quiz.Name))
            {
                throw new ItemNotFoundException();
            }
            if (!model.Quiz.Publish)
            {
                throw new ArgumentException("Quiz not published");
            }
            return model;
        }

        [Ajax, HttpGet]
        [ZboxAuthorize]
        [DonutOutputCache(Duration = TimeConsts.Hour, VaryByParam = "None", VaryByCustom = CustomCacheKeys.Auth + ";"
            + CustomCacheKeys.Lang)]
        public ActionResult CreateQuiz()
        {
            return PartialView("CreateQuiz");
        }

        [Ajax, HttpPost]
        [ZboxAuthorize]
        public ActionResult SaveAnswers(SaveUserAnswers model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var command = new SaveUserQuizCommand(model.Answers.Select(s => new UserAnswers { AnswerId = s.AnswerId, QuestionId = s.QuestionId }),
                GetUserId(), model.QuizId, model.EndTime - model.StartTime);
            m_ZboxWriteService.SaveUserAnswers(command);

            return this.CdJson(new JsonResponse(true));
        }

        [Ajax, HttpGet]
        [ZboxAuthorize]
        public async Task<ActionResult> GetDraft(long quizId)
        {
            //TODO:add validation -- only draft
            var query = new GetQuizDraftQuery(quizId);
            var values = await m_ZboxReadService.GetDraftQuiz(query);
            if (values.Publish)
            {
                throw new ArgumentException("Quiz is published");
            }
            if (values.OwnerId != GetUserId())
            {
                throw new ArgumentException("This is not the owner");
            }
            return this.CdJson(new JsonResponse(true, values));
        }

        #region Quiz
        [HttpPost, Ajax]
        [ZboxAuthorize]
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
        [ZboxAuthorize]
        public ActionResult Update(UpdateQuiz model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var command = new UpdateQuizCommand(GetUserId(), model.Id, model.Name);
            m_ZboxWriteService.UpdateQuiz(command);
            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult Delete(long id)
        {
            try
            {
                var command = new DeleteQuizCommand(id, GetUserId());
                m_ZboxWriteService.DeleteQuiz(command);
                return this.CdJson(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Delete quiz id:" + id, ex);
                return this.CdJson(new JsonResponse(false));
            }
        }

        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult Save(SaveQuiz model)
        {
            try
            {
                var command = new SaveQuizCommand(GetUserId(), model.QuizId);
                m_ZboxWriteService.SaveQuiz(command);
                var urlBuilder = new UrlBuilder(HttpContext);
                var url = urlBuilder.BuildQuizUrl(model.BoxId, model.BoxName, model.QuizId, model.QuizName, model.UniversityName);
                //TODO add url
                return this.CdJson(new JsonResponse(true, url));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Quiz/Save model: " + model, ex);
                return this.CdJson(new JsonResponse(false, ex.Message));
            }
        }
        #endregion

        #region question
        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult CreateQuestion(Question model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            if (model.QuizId == 0)
            {
                ModelState.AddModelError(string.Empty, "Quiz id cannot be 0");
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));

            }
            var id = m_IdGenerator.Value.GetId();
            var command = new CreateQuestionCommand(model.Text, model.QuizId, GetUserId(), id);
            m_ZboxWriteService.CreateQuestion(command);
            return this.CdJson(new JsonResponse(true, id));
        }
        [HttpPost, Ajax]
        [ZboxAuthorize]
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
        [ZboxAuthorize]
        public ActionResult DeleteQuestion(Guid id)
        {
            var command = new DeleteQuestionCommand(GetUserId(), id);
            m_ZboxWriteService.DeleteQuestion(command);
            return this.CdJson(new JsonResponse(true));
        }
        #endregion

        #region Answer
        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult CreateAnswer(Answer model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var id = m_IdGenerator.Value.GetId();
            var command = new CreateAnswerCommand(GetUserId(), id, model.Text, model.QuestionId);
            m_ZboxWriteService.CreateAnswer(command);
            return this.CdJson(new JsonResponse(true, id));
        }
        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult UpdateAnswer(UpdateAnswer model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }

            var command = new UpdateAnswerCommand(GetUserId(), model.Text, model.Id);
            m_ZboxWriteService.UpdateAnswer(command);
            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax, ZboxAuthorize]
        public ActionResult MarkCorrect(MarkAnswer model)
        {
            
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            if (!model.AnswerId.HasValue)
            {
                return this.CdJson(new JsonResponse(false, "Guid is empty"));
            }
            try
            {
                var command = new MarkAnswerCorrectCommand(model.AnswerId.Value, GetUserId());
                m_ZboxWriteService.MarkAnswerAsCorrect(command);
                return this.CdJson(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On mark answer", ex);
                return this.CdJson(new JsonResponse(false));
            }
        }

        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult DeleteAnswer(Guid id)
        {
            var command = new DeleteAnswerCommand(GetUserId(), id);
            m_ZboxWriteService.DeleteAnswer(command);
            return this.CdJson(new JsonResponse(true));
        }


        #endregion

        #region Discussion
        [HttpPost, Ajax, ZboxAuthorize]
        public ActionResult CreateDiscussion(Discussion model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var id = m_IdGenerator.Value.GetId();
            var command = new CreateDiscussionCommand(GetUserId(), model.Text, model.QuestionId, id);
            m_ZboxWriteService.CreateItemInDiscussion(command);
            return this.CdJson(new JsonResponse(true, id));
        }

        [HttpPost, Ajax, ZboxAuthorize]
        public ActionResult DeleteDiscussion(Guid id)
        {
            var command = new DeleteDiscussionCommand(id, GetUserId());
            m_ZboxWriteService.DeleteItemInDiscussion(command);
            return this.CdJson(new JsonResponse(true, id));

        }

        #endregion
    }
}