using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using System.Threading.Tasks;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    [Authorize]
    public class QuizController : ApiController
    {
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IQueueProvider m_QueueProvider;

        public QuizController(IZboxWriteService zboxWriteService, IGuidIdGenerator guidGenerator, IZboxCacheReadService zboxReadService, IQueueProvider queueProvider)
        {
            m_ZboxWriteService = zboxWriteService;
            m_GuidGenerator = guidGenerator;
            m_ZboxReadService = zboxReadService;
            m_QueueProvider = queueProvider;
        }


        public async Task<HttpResponseMessage> Post(SaveUserAnswersRequests model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            if (model.Answers == null)
            {
                return Request.CreateBadRequestResponse("Answers is required");
            }
            var command =
                new SaveUserQuizCommand(
                  model.Answers.Select(s => new UserAnswers { AnswerId = s.AnswerId, QuestionId = s.QuestionId }),
                    User.GetUserId(), model.QuizId, TimeSpan.FromSeconds(model.NumberOfSeconds), model.BoxId);
            await m_ZboxWriteService.SaveUserAnswersAsync(command);

            return Request.CreateResponse(string.Empty);
        }

        public async Task<HttpResponseMessage> Get(long boxId, long quizId)
        {
            var userId = User.GetUserId();
            var query = new GetQuizQuery(quizId, userId);
            var tModel = m_ZboxReadService.GetQuizAsync(query);
            var tTransaction = m_QueueProvider.InsertMessageToTransactionAsync(
                 new StatisticsData4(
                        new StatisticsData4.StatisticItemData
                        {
                            Id = quizId,
                            Action = (int)Zbox.Infrastructure.Enums.StatisticsAction.Quiz
                        }
                    , userId));

            await Task.WhenAll(tModel, tTransaction);
            return Request.CreateResponse(new
            {
                Question = tModel.Result.Quiz.Questions.Select(s => new
                {
                    s.Id,
                    s.Text,
                    s.CorrectAnswer,
                    Answers = s.Answers.Select(v => new
                    {
                        v.Id,
                        v.Text
                    })
                }
                ),
                Answers = tModel.Result.Sheet?.Questions,
                tModel.Result.Sheet,
                tModel.Result.Like
            });
        }

        [Route("api/quiz/{id:long}/solvers"), HttpGet]
        public async Task<HttpResponseMessage> GetQuizSolversAsync(long id)
        {
            var query = new GetQuizBestSolvers(id, 4);
            var retVal = await m_ZboxReadService.GetQuizSolversAsync(query);
            return Request.CreateResponse(new
            {
                retVal.SolversCount,
                Users = retVal.Users.Select(s => new
                {
                    s.Image
                })
            });
        }

        [Route("api/quiz/{id:long}/discussion"), HttpGet]
        public async Task<HttpResponseMessage> DiscussionAsync(long id)
        {
            var query = new GetDisscussionQuery(id);
            var model = await m_ZboxReadService.GetDiscussionAsync(query);
            return Request.CreateResponse(model.Select(s => new
            {
                s.QuestionId,
                s.Text,
                s.UserId,
                s.UserPicture,
                s.Date,
                s.UserName,
                s.Id
            }));
        }


        [Route("api/quiz/{id:long}/discussion"), HttpPost]
        // ReSharper disable once ConsiderUsingAsyncSuffix
        public async Task<HttpResponseMessage> PostDiscussion(DiscussionRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var id = m_GuidGenerator.GetId();
            var command = new CreateDiscussionCommand(User.GetUserId(), model.Text, model.QuestionId, id);
            await m_ZboxWriteService.CreateItemInDiscussionAsync(command);
            return Request.CreateResponse(id);
        }


        [Route("api/quiz/{id:long}/discussion"), HttpDelete]
        public HttpResponseMessage DeleteDiscussion(Guid id)
        {
            var command = new DeleteDiscussionCommand(id, User.GetUserId());
            m_ZboxWriteService.DeleteItemInDiscussion(command);
            return Request.CreateResponse(string.Empty);
        }

        [HttpPost, Route("api/quiz/{id:long}/like")]
        public async Task<HttpResponseMessage> AddLikeAsync(long id)
        {
            var command = new AddQuizLikeCommand(User.GetUserId(), id);
            await m_ZboxWriteService.AddQuizLikeAsync(command);
            return Request.CreateResponse(HttpStatusCode.OK, command.Id);
        }

        [HttpDelete, Route("api/quiz/{id:long}/like")]
        public async Task<HttpResponseMessage> DeleteLikeAsync(Guid likeId)
        {
            var command = new DeleteQuizLikeCommand(User.GetUserId(), likeId);
            await m_ZboxWriteService.DeleteQuizLikeAsync(command);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }
    }
}
