using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Threading.Tasks;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
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
                return Request.CreateBadRequestResponse("Answers is requeried");
            }
            var command =
                new SaveUserQuizCommand(
                  model.Answers.Select(s => new UserAnswers { AnswerId = s.AnswerId, QuestionId = s.QuestionId }),
                    User.GetCloudentsUserId(), model.QuizId, model.EndTime - model.StartTime, model.BoxId);
            await ZboxWriteService.SaveUserAnswersAsync(command);

            return Request.CreateResponse();
        }

        public async Task<HttpResponseMessage> Get(long boxId, long quizId)
        {
            var userId = User.GetCloudentsUserId();
            var query = new GetQuizQuery(quizId, userId, boxId);
            var tModel = ZboxReadService.GetQuiz(query);

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
            return Request.CreateResponse();
        }
    }
}
