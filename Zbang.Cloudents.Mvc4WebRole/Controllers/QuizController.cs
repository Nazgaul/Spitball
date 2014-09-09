﻿using System;
using System.Linq;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.Quiz;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.ViewModel.Queries;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using System.Web.UI;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class QuizController : BaseController
    {
        private readonly Lazy<IIdGenerator> m_IdGenerator;

        public QuizController(Lazy<IIdGenerator> idGenerator)
        {
            m_IdGenerator = idGenerator;
        }

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [NonAjax]
        [UserNavNWelcome]
        [NoCache]
        public async Task<ActionResult> IndexDesktop(long boxId, long quizId, string quizName, string universityName,
            string boxName)
        {
            try
            {
                var query = new GetQuizSeoQuery(quizId);

                var model = await ZboxReadService.GetQuizSeo(query);

                if (model == null)
                {
                    throw new ItemNotFoundException();
                }
                if (Request.Url != null && model.Url != Server.UrlDecode(Request.Url.AbsolutePath))
                {
                    throw new ItemNotFoundException();
                }

                if (string.IsNullOrEmpty(model.Country)) return View("Empty");

                

                var culture = Languages.GetCultureBaseOnCountry(model.Country);
                BaseControllerResources.Culture = culture;
                ViewBag.title = string.Format("{0} {1} | {2} {3} | {4} | {5}", BaseControllerResources.QuizTitlePrefix,
                    model.Name,
                    BaseControllerResources.QuizTitleText,
                    model.BoxName,
                    model.UniversityName,
                    BaseControllerResources.Cloudents);
                ViewBag.metaDescription = model.FirstQuestion;
                return View("Empty");
            }
            catch (ItemNotFoundException)
            {

                return RedirectToAction("index", "error");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("quiz ", ex);
                return RedirectToAction("index", "error");
            }
        }


        //[ZboxAuthorize(IsAuthenticationRequired = false)]
        //[NonAjax]
        //[OutputCache(CacheProfile = "NoCache")]
        //public async Task<ActionResult> Index(long boxId, long quizId, string quizName, string universityName, string boxName)
        //{

        //    var model = await GetQuiz(boxId, quizId, quizName, true);

        //    var serializer = new JsonNetSerializer();
        //    ViewBag.userD = serializer.Serialize(model.Sheet);
        //    ViewBag.boxName = boxName;
        //    ViewBag.boxUrl = model.Quiz.Seo.BoxUrl;


        //    if (model.Quiz.Seo != null && !string.IsNullOrEmpty(model.Quiz.Seo.Country))
        //    {
        //        var culture = Languages.GetCultureBaseOnCountry(model.Quiz.Seo.Country);
        //        BaseControllerResources.Culture = culture;
        //        ViewBag.title = string.Format("{0} {1} | {2} {3} | {4} | {5}", BaseControllerResources.QuizTitlePrefix, model.Quiz.Name, BaseControllerResources.QuizTitleText, model.Quiz.Seo.BoxName, model.Quiz.Seo.UniversityName, BaseControllerResources.Cloudents);
        //    }
        //    ViewBag.metaDescription = model.Quiz.Questions.First().Text;

        //    return View(model.Quiz);
        //}
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [Ajax]
        [ActionName("Index")]
        [NoCache]
        //TODO: put cache
        public ActionResult IndexAjax()
        {
            return PartialView();

        }

        [HttpGet]
        [Ajax]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public async Task<ActionResult> Data(long boxId, long quizId)
        {
            var query = new GetQuizQuery(quizId, GetUserId(false), boxId);
            var model = await ZboxReadService.GetQuiz(query);
            return Json(new JsonResponse(true, model));
        }

        [Ajax]
        [ZboxAuthorize]
        public async Task<ActionResult> Discussion(long quizId)
        {
            var query = new GetDisscussionQuery(quizId);
            var model = await ZboxReadService.GetDiscussion(query);
            return Json(new JsonResponse(true, model));
        }


        [HttpGet, Ajax]
        //[OutputCache(Duration = TimeConsts.Hour, 
        //    Location = OutputCacheLocation.Any, VaryByParam = "none",
        //    VaryByCustom = CustomCacheKeys.Lang)]
        public async Task<ActionResult> ChallengePartial(long quizId)
        {
            try
            {
                var numberOfSolvers = await ZboxReadService.GetNumberOfSolvers(quizId);
                return PartialView("_QuizDialog", numberOfSolvers);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_QuizDialog", ex);
                return Json(new JsonResponse(false));
            }
        }
        //[NonAction]
        //private async Task<QuizWithDetailSolvedDto> GetQuiz(long boxId, long quizId, string quizName)
        //{
        //    var query = new GetQuizQuery(quizId, GetUserId(false), boxId);
        //    var model = await ZboxReadService.GetQuiz(query);
        //    if (model.Quiz.BoxId != boxId)
        //    {
        //        throw new ItemNotFoundException();
        //    }
        //    if (quizName != UrlBuilder.NameToQueryString(model.Quiz.Name))
        //    {
        //        throw new ItemNotFoundException();
        //    }
        //    if (!model.Quiz.Publish)
        //    {
        //        throw new ArgumentException("Quiz not published");
        //    }
        //    if (model.Sheet != null)
        //    {
        //        if (model.Sheet.Stats != null && model.Sheet.Stats.Stdevp != 0)
        //        {
        //            model.Sheet.Stats.UserPosition = (model.Sheet.Score - model.Sheet.Stats.Avg) / model.Sheet.Stats.Stdevp;
        //        }
        //    }
        //    return model;
        //}

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
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            try
            {
                var command =
                    new SaveUserQuizCommand(
                        model.Answers.Select(s => new UserAnswers { AnswerId = s.AnswerId, QuestionId = s.QuestionId }),
                        GetUserId(), model.QuizId, model.EndTime - model.StartTime);
                ZboxWriteService.SaveUserAnswers(command);

                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Save answers model: {0}", model), ex);
                return Json(new JsonResponse(false));
            }
        }

        [Ajax, HttpGet]
        [ZboxAuthorize]
        public async Task<ActionResult> GetDraft(long quizId)
        {
            //TODO:add validation -- only draft
            var query = new GetQuizDraftQuery(quizId);
            var values = await ZboxReadService.GetDraftQuiz(query);
            if (values.Publish)
            {
                throw new ArgumentException("Quiz is published");
            }
            if (values.OwnerId != GetUserId())
            {
                throw new ArgumentException("This is not the owner");
            }
            return Json(new JsonResponse(true, values));
        }

        #region Quiz
        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult Create(Quiz model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var id = m_IdGenerator.Value.GetId(IdGenerator.QuizScope);
            var command = new CreateQuizCommand(GetUserId(), id, model.Name, model.BoxId);
            ZboxWriteService.CreateQuiz(command);

            return Json(new JsonResponse(true, id));
        }

        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult Update(UpdateQuiz model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var command = new UpdateQuizCommand(GetUserId(), model.Id, model.Name);
            ZboxWriteService.UpdateQuiz(command);
            return Json(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult Delete(long id)
        {
            try
            {
                var command = new DeleteQuizCommand(id, GetUserId());
                ZboxWriteService.DeleteQuiz(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Delete quiz id:" + id + " userid: " + GetUserId(), ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult Save(SaveQuiz model)
        {
            try
            {
                var command = new SaveQuizCommand(GetUserId(), model.QuizId);
                var result = ZboxWriteService.SaveQuiz(command);
                return Json(new JsonResponse(true, result));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Quiz/Save model: " + model, ex);
                return Json(new JsonResponse(false, ex.Message));
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
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            if (model.QuizId == 0)
            {
                ModelState.AddModelError(string.Empty, "Quiz id cannot be 0");
                return Json(new JsonResponse(false, GetErrorsFromModelState()));

            }
            var id = m_IdGenerator.Value.GetId();
            var command = new CreateQuestionCommand(model.Text, model.QuizId, GetUserId(), id);
            ZboxWriteService.CreateQuestion(command);
            return Json(new JsonResponse(true, id));
        }
        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult UpdateQuestion(UpdateQuestion model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var command = new UpdateQuestionCommand(GetUserId(), model.Id, model.Text);
            ZboxWriteService.UpdateQuestion(command);
            return Json(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult DeleteQuestion(Guid id)
        {
            var command = new DeleteQuestionCommand(GetUserId(), id);
            ZboxWriteService.DeleteQuestion(command);
            return Json(new JsonResponse(true));
        }
        #endregion

        #region Answer
        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult CreateAnswer(Answer model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            if (model.QuestionId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, @"No Question Given");
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var id = m_IdGenerator.Value.GetId();
            var command = new CreateAnswerCommand(GetUserId(), id, model.Text, model.QuestionId);
            ZboxWriteService.CreateAnswer(command);
            return Json(new JsonResponse(true, id));
        }
        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult UpdateAnswer(UpdateAnswer model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }

            var command = new UpdateAnswerCommand(GetUserId(), model.Text, model.Id);
            ZboxWriteService.UpdateAnswer(command);
            return Json(new JsonResponse(true));
        }
        [HttpPost, Ajax, ZboxAuthorize]
        public ActionResult MarkCorrect(MarkAnswer model)
        {

            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            if (!model.AnswerId.HasValue)
            {
                return Json(new JsonResponse(false, "Guid is empty"));
            }
            try
            {
                var command = new MarkAnswerCorrectCommand(model.AnswerId.Value, GetUserId());
                ZboxWriteService.MarkAnswerAsCorrect(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On mark answer", ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult DeleteAnswer(Guid id)
        {
            var command = new DeleteAnswerCommand(GetUserId(), id);
            ZboxWriteService.DeleteAnswer(command);
            return Json(new JsonResponse(true));
        }


        #endregion

        #region Discussion
        [HttpPost, Ajax, ZboxAuthorize]
        public ActionResult CreateDiscussion(Discussion model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var id = m_IdGenerator.Value.GetId();
            var command = new CreateDiscussionCommand(GetUserId(), model.Text, model.QuestionId, id);
            ZboxWriteService.CreateItemInDiscussion(command);
            return Json(new JsonResponse(true, id));
        }

        [HttpPost, Ajax, ZboxAuthorize]
        public ActionResult DeleteDiscussion(Guid id)
        {
            var command = new DeleteDiscussionCommand(id, GetUserId());
            ZboxWriteService.DeleteItemInDiscussion(command);
            return Json(new JsonResponse(true, id));

        }

        #endregion
    }
}