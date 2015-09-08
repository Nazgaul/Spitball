using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Threading.Tasks;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class QuizController : ApiController
    {
        public IZboxWriteService ZboxWriteService { get; set; }


        public  IGuidIdGenerator GuidGenerator {get;set;}

        public ApiServices Services { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }

        public IQueueProvider QueueProvider { get; set; }

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
                    User.GetCloudentsUserId(), model.QuizId, TimeSpan.FromSeconds(model.NumberOfSeconds), model.BoxId);
            await ZboxWriteService.SaveUserAnswersAsync(command);

            return Request.CreateResponse();
        }

        public async Task<HttpResponseMessage> Get(long boxId, long quizId)
        {
            var userId = User.GetCloudentsUserId();
            var query = new GetQuizQuery(quizId, userId, boxId);
            var tModel = ZboxReadService.GetQuizQuestionWithAnswersAsync(query);

            var tTransaction = QueueProvider.InsertMessageToTranactionAsync(
                 new StatisticsData4(new List<StatisticsData4.StatisticItemData>
                    {
                        new StatisticsData4.StatisticItemData
                        {
                            Id = quizId,
                            Action = (int)Zbox.Infrastructure.Enums.StatisticsAction.Quiz
                        }
                    }, userId, DateTime.UtcNow));

            await Task.WhenAll(tModel, tTransaction);
            return Request.CreateResponse(new
            {
                Question = tModel.Result.Questions.Select(s => new
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
                Answers = tModel.Result.UserAnswers
            });
        }

        [Route("api/quiz/{id:long}/solvers"), HttpGet]
        public async Task<HttpResponseMessage> GetQuizSolvers(long id)
        {
            var query = new GetQuizBestSolvers(id, 4);
            var retVal = await ZboxReadService.GetQuizSolversAsync(query);
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
        public async Task<HttpResponseMessage> Discussion(long id)
        {
            var query = new GetDisscussionQuery(id);
            var model = await ZboxReadService.GetDiscussion(query);
            return Request.CreateResponse(model.Select(s => new
            {
                s.QuestionId,
                s.Text,
                s.UserId,
                s.UserPicture,
                s.Date,
                s.UserName
            }));
        }


        [Route("api/quiz/{id:long}/discussion"), HttpPost]
        public HttpResponseMessage PostDiscussion(DiscussionRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var id = GuidGenerator.GetId();
            var command = new CreateDiscussionCommand(User.GetCloudentsUserId(), model.Text, model.QuestionId, id);
            ZboxWriteService.CreateItemInDiscussion(command);
            return Request.CreateResponse();
        }


        [Route("api/quiz/{id:long}/discussion"), HttpDelete]
        public HttpResponseMessage PostDiscussion(Guid id)
        {
            var command = new DeleteDiscussionCommand(id, User.GetCloudentsUserId());
            ZboxWriteService.DeleteItemInDiscussion(command);
            return Request.CreateResponse();
        }
    }
}
