﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.Quiz;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
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
        private readonly IIdGenerator m_IdGenerator;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IGuidIdGenerator m_GuidGenerator;

        public QuizController(IIdGenerator idGenerator, IQueueProvider queueProvider, IGuidIdGenerator guidGenerator)
        {
            m_IdGenerator = idGenerator;
            m_QueueProvider = queueProvider;
            m_GuidGenerator = guidGenerator;
        }

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        //[UserNavNWelcome]
        [NoCache]
        [BoxPermission("boxId", Order = 2)]
        [RedirectToMobile(Order = 1)]
        [DonutOutputCache(VaryByCustom = CustomCacheKeys.Lang,
         Duration = TimeConsts.Hour * 1, VaryByParam = "quizId",
         Location = OutputCacheLocation.Server, Order = 4)]
        public async Task<ActionResult> Index(long boxId, long quizId, string quizName, string universityName,
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
                ViewBag.title = string.Format("{0} | {1} | {2} | {3} | {4}",
                   model.BoxName, model.Name, model.DepartmentName, model.UniversityName, BaseControllerResources.Cloudents);

                ViewBag.metaDescription = Zbox.Infrastructure.TextManipulation.RemoveHtmlTags.Replace(model.FirstQuestion, string.Empty);
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



        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult IndexPartial()
        {
            return PartialView("Index2");

        }

        [ZboxAuthorize]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult CreatePartial()
        {
            return PartialView("CreateQuiz");
        }

        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("boxId")]
        public async Task<ActionResult> Data(long boxId, long quizId)
        {
            var userId = User.GetUserId(false);
            var query = new GetQuizQuery(quizId, userId, boxId);
            var tModel = ZboxReadService.GetQuiz(query);

            var tTransaction = m_QueueProvider.InsertMessageToTranactionAsync(
                 new StatisticsData4(new List<StatisticsData4.StatisticItemData>
                    {
                        new StatisticsData4.StatisticItemData
                        {
                            Id = quizId,
                            Action = (int)Zbox.Infrastructure.Enums.StatisticsAction.Quiz
                        }
                    }, userId, DateTime.UtcNow));

            await Task.WhenAll(tModel, tTransaction);
            return Json(new JsonResponse(true, tModel.Result));
        }

        [ZboxAuthorize]
        [HttpGet]
        //TODO: add validation in here
        public async Task<JsonResult> Discussion(long quizId)
        {
            var query = new GetDisscussionQuery(quizId);
            var model = await ZboxReadService.GetDiscussion(query);
            return Json(new JsonResponse(true, model));
        }


        //TODO: add validation in here
        [HttpGet]
        //[OutputCache(Duration = TimeConsts.Hour, 
        //    Location = OutputCacheLocation.Any, VaryByParam = "none",
        //    VaryByCustom = CustomCacheKeys.Lang)]
        public async Task<ActionResult> NumberOfSolvers(long quizId)
        {
            try
            {
                var numberOfSolvers = await ZboxReadService.GetNumberOfSolvers(quizId);
                return JsonOk(numberOfSolvers);
                //return PartialView("_QuizDialog", numberOfSolvers);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_QuizDialog", ex);
                return Json(new JsonResponse(false));
            }
        }


        [HttpGet]
        [ZboxAuthorize]
        [OutputCache(CacheProfile = "PartialCache")]


        public ActionResult CreateQuiz()
        {
            return PartialView("CreateQuiz");
        }

        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie]
        public async Task<JsonResult> SaveAnswers(SaveUserAnswers model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorsFromModelState());
            }
            if (model.Answers == null)
            {
                ModelState.AddModelError(string.Empty, "Answers is requeried");
                return JsonError(GetErrorsFromModelState());
            }
            try
            {
                var command =
                    new SaveUserQuizCommand(
                      model.Answers.Select(s => new UserAnswers { AnswerId = s.AnswerId, QuestionId = s.QuestionId }),
                        User.GetUserId(), model.QuizId, TimeSpan.FromMilliseconds(model.NumberOfMilliseconds), model.BoxId);
                await ZboxWriteService.SaveUserAnswersAsync(command);

                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Save answers model: {0}", model), ex);
                return JsonError();
            }
        }

        [HttpGet]
        [ZboxAuthorize]
        public async Task<ActionResult> GetDraft(long quizId)
        {
            var query = new GetQuizDraftQuery(quizId);
            var values = await ZboxReadService.GetDraftQuiz(query);
            if (values.Publish)
            {
                throw new ArgumentException("Quiz is published");
            }
            if (values.OwnerId != User.GetUserId())
            {
                throw new ArgumentException("This is not the owner");
            }
            return Json(new JsonResponse(true, values));
        }

        #region Quiz
        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie]
        public async Task<ActionResult> Create(Quiz model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorsFromModelState());
            }
            var id = m_IdGenerator.GetId(IdContainer.QuizScope);
            var command = new CreateQuizCommand(User.GetUserId(), id, model.Name, model.BoxId);
            await ZboxWriteService.CreateQuizAsync(command);

            return JsonOk(id);
        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult Update(UpdateQuiz model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var command = new UpdateQuizCommand(User.GetUserId(), model.Id, model.Name);
            ZboxWriteService.UpdateQuiz(command);
            return Json(new JsonResponse(true));
        }
        [HttpPost]
        [ZboxAuthorize]
        public ActionResult Delete(long id)
        {
            try
            {
                var command = new DeleteQuizCommand(id, User.GetUserId());
                ZboxWriteService.DeleteQuiz(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Delete quiz id:" + id + " userid: " + User.GetUserId(), ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpPost]
        [ZboxAuthorize]
        public async Task<JsonResult> Save(SaveQuiz model)
        {
            try
            {
                var command = new SaveQuizCommand(User.GetUserId(), model.QuizId);
                var result = await ZboxWriteService.SaveQuizAsync(command);
                return JsonOk(result);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Quiz/Save model: " + model, ex);
                return JsonError(ex.Message);
            }
        }
        #endregion

        #region question
        [HttpPost]
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
            var id = m_GuidGenerator.GetId();
            var command = new CreateQuestionCommand(model.Text, model.QuizId, User.GetUserId(), id);
            ZboxWriteService.CreateQuestion(command);
            return Json(new JsonResponse(true, id));
        }
        [HttpPost]
        [ZboxAuthorize]
        public ActionResult UpdateQuestion(UpdateQuestion model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var command = new UpdateQuestionCommand(User.GetUserId(), model.Id, model.Text);
            ZboxWriteService.UpdateQuestion(command);
            return Json(new JsonResponse(true));
        }
        [HttpPost]
        [ZboxAuthorize]
        public ActionResult DeleteQuestion(Guid id)
        {
            try
            {
                var command = new DeleteQuestionCommand(User.GetUserId(), id);
                ZboxWriteService.DeleteQuestion(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Delete Question id: " + id, ex);
                return Json(new JsonResponse(false));
            }
        }
        #endregion

        #region Answer
        [HttpPost]
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
            var id = m_GuidGenerator.GetId();
            var command = new CreateAnswerCommand(User.GetUserId(), id, model.Text, model.QuestionId);
            ZboxWriteService.CreateAnswer(command);
            return Json(new JsonResponse(true, id));
        }
        [HttpPost]
        [ZboxAuthorize]
        public ActionResult UpdateAnswer(UpdateAnswer model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }

            var command = new UpdateAnswerCommand(User.GetUserId(), model.Text, model.Id);
            ZboxWriteService.UpdateAnswer(command);
            return Json(new JsonResponse(true));
        }
        [HttpPost, ZboxAuthorize]
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
                var command = new MarkAnswerCorrectCommand(model.AnswerId.Value, User.GetUserId());
                ZboxWriteService.MarkAnswerAsCorrect(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On mark answer", ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult DeleteAnswer(Guid id)
        {
            var command = new DeleteAnswerCommand(User.GetUserId(), id);
            ZboxWriteService.DeleteAnswer(command);
            return Json(new JsonResponse(true));
        }


        #endregion

        #region Discussion
        [HttpPost, ZboxAuthorize]
        public ActionResult CreateDiscussion(Discussion model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var id = m_GuidGenerator.GetId();
            var command = new CreateDiscussionCommand(User.GetUserId(), model.Text, model.QuestionId, id);
            ZboxWriteService.CreateItemInDiscussion(command);
            return Json(new JsonResponse(true, id));
        }

        [HttpPost, ZboxAuthorize]
        public ActionResult DeleteDiscussion(Guid id)
        {
            var command = new DeleteDiscussionCommand(id, User.GetUserId());
            ZboxWriteService.DeleteItemInDiscussion(command);
            return Json(new JsonResponse(true, id));

        }

        #endregion
    }
}