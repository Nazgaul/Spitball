using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class QuizController : ApiController
    {
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IQueueProvider m_QueueProvider;

        public QuizController(IQueueProvider queueProvider, IZboxCacheReadService zboxReadService)
        {
            m_QueueProvider = queueProvider;
            m_ZboxReadService = zboxReadService;
        }

        // GET api/Quiz
        public async Task<HttpResponseMessage> Get(long boxId, long quizId)
        {
            var userId = User.GetUserId();
            var query = new GetQuizQuery(quizId, userId, boxId);
            var tModel = m_ZboxReadService.GetQuizAsync(query);
            var tTransaction = m_QueueProvider.InsertMessageToTranactionAsync(
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
                Answers = tModel.Result.Sheet.Questions,
                tModel.Result.Sheet,
                tModel.Result.Like
            });
        }
    }
}
