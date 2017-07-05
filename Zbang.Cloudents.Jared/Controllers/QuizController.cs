using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
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
        private readonly IZboxWriteService m_ZboxWriteService;

        public QuizController(IQueueProvider queueProvider, IZboxCacheReadService zboxReadService, IZboxWriteService zboxWriteService)
        {
            m_QueueProvider = queueProvider;
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
        }

        // GET api/Quiz
        // ReSharper disable once ConsiderUsingAsyncSuffix
        [Authorize]
        public async Task<HttpResponseMessage> Get(long quizId)
        {
            var userId = User.GetUserId();
            var query = new GetQuizQuery(quizId, userId);
            var tModel = m_ZboxReadService.GetQuizAsync(query);
            var tTransaction = m_QueueProvider.InsertMessageToTransactionAsync(
                 new StatisticsData4(
                        new StatisticsData4.StatisticItemData
                        {
                            Id = quizId,
                            Action = (int)StatisticsAction.Quiz
                        }
                    , userId));

            await Task.WhenAll(tModel, tTransaction).ConfigureAwait(false);
            if (tModel.Result == null)
            {
                return Request.CreateNotFoundResponse();
            }
            return Request.CreateResponse(new
            {
                tModel.Result.Quiz.Name,
                Questions = tModel.Result.Quiz.Questions.Select(s => new
                {
                    s.Id,
                    s.Text,
                    s.CorrectAnswer,
                    Answers = s.Answers.Select(v => new
                    {
                        v.Id,
                        v.Text
                    })
                }),
                tModel.Result.Like
            });
        }

        [Route("api/quiz/{id:long}/discussion"), HttpGet]
        public async Task<HttpResponseMessage> DiscussionAsync(long id)
        {
            var query = new GetDisscussionQuery(id);
            var model = await m_ZboxReadService.GetDiscussionAsync(query).ConfigureAwait(false);
            return Request.CreateResponse(model.Select(s => new
            {
                s.QuestionId,
                s.Text,
                //s.UserId,
                s.UserPicture,
                s.Date,
                s.UserName,
                //s.Id
            }));
        }

        [HttpPost, Route("api/quiz/like")]
        [Authorize]
        public async Task<HttpResponseMessage> AddLikeAsync(ItemLikeRequest model)
        {
            var command = new AddQuizLikeCommand(User.GetUserId(), model.Id);
            await m_ZboxWriteService.AddQuizLikeAsync(command).ConfigureAwait(true);

            if (model.Tags != null && model.Tags.Any())
            {
                var z = new AssignTagsToQuizCommand(model.Id, model.Tags, TagType.User);
                await m_ZboxWriteService.AddItemTagAsync(z).ConfigureAwait(false);
            }

            return Request.CreateResponse(HttpStatusCode.OK, command.Id);
        }
        [HttpDelete, Route("api/quiz/like")]
        [Authorize]
        public async Task<HttpResponseMessage> DeleteLikeAsync(Guid likeId)
        {
            var command = new DeleteQuizLikeCommand(User.GetUserId(), likeId);
            await m_ZboxWriteService.DeleteQuizLikeAsync(command).ConfigureAwait(true);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //[Route("api/quiz/tag")]
        //[HttpPost]
        ////[Authorize]
        //public HttpResponseMessage AddTag(ItemTagRequest model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateBadRequestResponse();
        //    }

        //    var z = new AssignTagsToQuizCommand(model.Id, model.Tags, TagType.User);
        //    m_ZboxWriteService.AddItemTag(z);
        //    return Request.CreateResponse();
        //}


    }
}
