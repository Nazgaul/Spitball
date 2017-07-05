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
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ViewModel.Queries;
using System.Threading.Tasks;
using System.Web.Routing;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using System.Web.UI;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Url;

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
        [NoCache]
        [BoxPermission("boxId", Order = 2)]
        [DonutOutputCache(VaryByCustom = CustomCacheKeys.Lang,
         Duration = TimeConst.Hour * 1, VaryByParam = "quizId",
         Location = OutputCacheLocation.Server, Order = 4)]
        [Route("quiz/{universityName}/{boxId:long}/{boxName}/{quizId:long}/{quizName}", Name = "Quiz")]
        public async Task<ActionResult> IndexAsync(long boxId, long quizId, string quizName, string universityName,
            string boxName)
        {
            try
            {
                var query = new GetQuizSeoQuery(quizId);

                var model = await ZboxReadService.GetQuizSeoAsync(query).ConfigureAwait(false);

                if (model == null)
                {
                    throw new ItemNotFoundException();
                }
                //if (Request.Url != null && model.Url != Server.UrlDecode(Request.Url.AbsolutePath))
                //{
                //    throw new ItemNotFoundException();
                //}

                if (string.IsNullOrEmpty(model.Country)) return View("Empty");
                SeoBaseUniversityResources.Culture = Languages.GetCultureBaseOnCountry(model.Country);
                ViewBag.title =
                    $"{SeoBaseUniversityResources.QuizTitle} - {model.Name} - {model.BoxName} | {SeoBaseUniversityResources.Cloudents}";

                ViewBag.metaDescription = string.Format(SeoBaseUniversityResources.QuizMetaDescription, model.BoxName, model.Name);
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

        [Route(UrlConst.ShortQuiz, Name = "shortQuiz"), NoUrlLowercase]
        public async Task<ActionResult> ShortUrlAsync(string flashcard62Id)
        {
            var base62 = new Base62(flashcard62Id);
            var query = new GetQuizSeoQuery(base62.Value);
            var model = await ZboxReadService.GetQuizSeoAsync(query).ConfigureAwait(false);
            if (model == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            //quiz/{universityName}/{boxId:long}/{boxName}/{quizId:long}/{quizName}
            var route = new RouteValueDictionary
            {
                ["universityName"] = UrlConst.NameToQueryString(model.UniversityName ?? "my"),
                ["boxId"] = model.BoxId,
                ["boxName"] = UrlConst.NameToQueryString(model.BoxName),
                ["flashcardId"] = base62.Value,
                ["flashcardName"] = UrlConst.NameToQueryString(model.Name)
            };
            if (Request.IsAjaxRequest())
            {
                return JsonOk(Url.RouteUrl("Quiz", route));
            }
            return RedirectToRoutePermanent("Quiz", route);
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
        [BoxPermission("boxId"), ActionName("Data")]
        public async Task<ActionResult> DataAsync(long boxId, long quizId)
        {
            var userId = User.GetUserId(false);
            var query = new GetQuizQuery(quizId, userId);
            var tModel = ZboxReadService.GetQuizAsync(query);

            var tTransaction = m_QueueProvider.InsertMessageToTransactionAsync(
                 new StatisticsData4(
                        new StatisticsData4.StatisticItemData
                        {
                            Id = quizId,
                            Action = (int)Zbox.Infrastructure.Enums.StatisticsAction.Quiz
                        }
                    , userId));

            await Task.WhenAll(tModel, tTransaction);
            return JsonOk(tModel.Result);
        }

        [ZboxAuthorize]
        [HttpGet, ActionName("Discussion")]
        //TODO: add validation in here
        public async Task<JsonResult> DiscussionAsync(long quizId)
        {
            var query = new GetDisscussionQuery(quizId);
            var model = await ZboxReadService.GetDiscussionAsync(query);
            return Json(new JsonResponse(true, model.Select(s => new
            {
                CreationTime = s.Date,
                s.Id,
                s.QuestionId,
                Content = s.Text,
                s.UserId,
                s.UserName,
                UserImage = s.UserPicture
            })));
        }


        [HttpGet, ActionName("NumberOfSolvers")]
        public async Task<ActionResult> NumberOfSolversAsync(long quizId)
        {
            try
            {
                var query = new GetQuizBestSolvers(quizId, 4);
                var retVal = await ZboxReadService.GetQuizSolversAsync(query);
                return JsonOk(new
                {
                    retVal.SolversCount,
                    retVal.Users
                });
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_QuizDialog", ex);
                return Json(new JsonResponse(false));
            }
        }


        [HttpGet]
        [ZboxAuthorize]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult CreateQuiz()
        {
            return PartialView("CreateQuiz");
        }

        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie, ActionName("SaveAnswers")]
        public async Task<JsonResult> SaveAnswersAsync(SaveUserAnswers model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            if (model.Answers == null)
            {
                ModelState.AddModelError(string.Empty, BaseControllerResources.QuizController_SaveAnswers_Answers_is_requeried);
                return JsonError(GetErrorFromModelState());
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
                TraceLog.WriteError($"Save answers model: {model}", ex);
                return JsonError();
            }
        }

        [HttpGet]
        [ZboxAuthorize, ActionName("Draft")]
        public async Task<ActionResult> DraftAsync(long quizId)
        {
            var query = new GetQuizDraftQuery(quizId);
            var values = await ZboxReadService.GetDraftQuizAsync(query);
            if (values.Publish)
            {
                throw new ArgumentException("Quiz is published");
            }
            if (values.OwnerId != User.GetUserId())
            {
                throw new ArgumentException("This is not the owner");
            }
            return JsonOk(new
            {
                values.Id,
                values.Questions,
                values.Name
            });
        }

        #region Quiz
        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie, ActionName("Create")]
        public async Task<JsonResult> CreateAsync(Quiz model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
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
                return JsonError(GetErrorFromModelState());
            }
            var command = new UpdateQuizCommand(User.GetUserId(), model.Id, model.Name);
            ZboxWriteService.UpdateQuiz(command);
            return JsonOk();
        }
        [HttpPost]
        [ZboxAuthorize, ActionName("Delete")]
        public async Task<JsonResult> DeleteAsync(long id)
        {
            try
            {
                var command = new DeleteQuizCommand(id, User.GetUserId());
                await ZboxWriteService.DeleteQuizAsync(command);
                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Delete quiz id:" + id + " userid: " + User.GetUserId(), ex);
                return JsonError();
            }
        }

        [HttpPost]
        [ZboxAuthorize, ActionName("Save")]
        public async Task<JsonResult> SaveAsync(SaveQuiz model)
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

        [HttpPost, ZboxAuthorize, ActionName("like")]
        public async Task<JsonResult> AddLikeAsync(long id)
        {
            var command = new AddQuizLikeCommand(User.GetUserId(), id);
            await ZboxWriteService.AddQuizLikeAsync(command);
            return JsonOk(command.Id);
        }
        [HttpDelete, ZboxAuthorize, ActionName("like")]
        public async Task<JsonResult> DeleteLikeAsync(Guid id)
        {
            var command = new DeleteQuizLikeCommand(User.GetUserId(), id);
            await ZboxWriteService.DeleteQuizLikeAsync(command);
            return JsonOk();
        }

        #region question
        [HttpPost]
        [ZboxAuthorize, ActionName("CreateQuestion")]
        public async Task<JsonResult> CreateQuestionAsync(long? quizId, Models.Quiz.Question model)
        {
            //return JsonOk(model);
            if (!quizId.HasValue)
            {
                ModelState.AddModelError(string.Empty, BaseControllerResources.QuizController_CreateQuestion_Quiz_id_cannot_be_0);

            }
            if (model.Id.HasValue)
            {
                ModelState.AddModelError(string.Empty, BaseControllerResources.UnspecifiedError);
            }
            if (model.Answers.Any(a => a.Id.HasValue))
            {
                ModelState.AddModelError(string.Empty, BaseControllerResources.UnspecifiedError);
            }
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            model.Id = m_GuidGenerator.GetId();
            foreach (var answer in model.Answers)
            {
                answer.Id = m_GuidGenerator.GetId();
                await Task.Delay(1);
            }
            var command = new CreateQuestionCommand(quizId.Value, User.GetUserId(),
                new Zbox.Domain.Commands.Quiz.Question(model.Id.Value,
                model.Text,
                model.Answers.Select((s, i) =>
                 new Zbox.Domain.Commands.Quiz.Answer(s.Id.Value, s.Text, i == model.CorrectAnswer))));
            ZboxWriteService.CreateQuestion(command);
            return JsonOk(model);
        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult DeleteQuestion(Guid id)
        {
            try
            {
                var command = new DeleteQuestionCommand(User.GetUserId(), id);
                ZboxWriteService.DeleteQuestion(command);
                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Delete Question id: " + id, ex);
                return JsonError();
            }
        }
        #endregion



        #region Discussion
        [HttpPost, ZboxAuthorize, ActionName("CreateDiscussion")]
        public async Task<JsonResult> CreateDiscussionAsync(Discussion model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var id = m_GuidGenerator.GetId();
            var command = new CreateDiscussionCommand(User.GetUserId(), model.Text, model.QuestionId, id);
            await ZboxWriteService.CreateItemInDiscussionAsync(command);
            return JsonOk(id);
        }

        [HttpPost, ZboxAuthorize]
        public ActionResult DeleteDiscussion(Guid id)
        {
            var command = new DeleteDiscussionCommand(id, User.GetUserId());
            ZboxWriteService.DeleteItemInDiscussion(command);
            return JsonOk(id);

        }

        #endregion
    }
}