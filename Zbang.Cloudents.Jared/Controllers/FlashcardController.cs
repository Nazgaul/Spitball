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
    public class FlashcardController : ApiController
    {
        private readonly IQueueProvider m_QueueProvider;
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IDocumentDbReadService m_DocumentDbReadService;
        private readonly IZboxWriteService m_ZboxWriteService;

        public FlashcardController(IQueueProvider queueProvider, 
            IZboxCacheReadService zboxReadService,
            IDocumentDbReadService documentDbReadService, IZboxWriteService zboxWriteService)
        {
            m_QueueProvider = queueProvider;
            m_ZboxReadService = zboxReadService;
            m_DocumentDbReadService = documentDbReadService;
            m_ZboxWriteService = zboxWriteService;
        }

        // GET api/Flashcard
        [Authorize]
        public async Task<HttpResponseMessage> Get(long id)
        {
            var userId = User.GetUserId();
            var tTransAction = m_QueueProvider.InsertMessageToTranactionAsync(
                      new StatisticsData4(
                        new StatisticsData4.StatisticItemData
                        {
                            Id = id,
                            Action = (int)StatisticsAction.Flashcard
                        }
                    , userId));

            var tUserValues = m_ZboxReadService.GetUserFlashcardAsync(new GetUserFlashcardQuery(
                    userId, id));
            var tValues = m_DocumentDbReadService.FlashcardAsync(id);
            await Task.WhenAll(tTransAction, tValues/*, tUserValues*/).ConfigureAwait(false);
            var values = tValues.Result;
            if (!values.Publish)
            {
                throw new ArgumentException("Flashcard is not published");
            }
            if (values.IsDeleted)
            {
                throw new ArgumentException("Flashcard is deleted");
            }
            return Request.CreateResponse(new
            {
                values.Cards,
                values.Name,
                //tUserValues.Result.Pins,
                tUserValues.Result.Like
            });
        }

        [HttpPost, Route("api/flashcard/like")]
        [Authorize]
        public async Task<HttpResponseMessage> AddLikeAsync(ItemLikeRequest model)
        {
            var command = new AddFlashcardLikeCommand(User.GetUserId(), model.Id);
            await m_ZboxWriteService.AddFlashcardLikeAsync(command).ConfigureAwait(false);

            if (model.Tags.Any())
            {
                var z = new AssignTagsToQuizCommand(model.Id, model.Tags, TagType.User);
                await m_ZboxWriteService.AddItemTagAsync(z).ConfigureAwait(false);
            }

            return Request.CreateResponse(HttpStatusCode.OK, command.Id);
        }
        [HttpDelete, Route("api/flashcard/like")]
        [Authorize]
        public async Task<HttpResponseMessage> DeleteLikeAsync(Guid likeId)
        {
            var command = new DeleteFlashcardLikeCommand(User.GetUserId(), likeId);
            await m_ZboxWriteService.DeleteFlashcardLikeAsync(command).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //[Route("api/flashcard/tag")]
        //[HttpPost]
        ////[Authorize]
        //public HttpResponseMessage AddTag(ItemTagRequest model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateBadRequestResponse();
        //    }

        //    var z = new AssignTagsToFlashcardCommand(model.Id, model.Tags, TagType.User);
        //    m_ZboxWriteService.AddItemTag(z);
        //    return Request.CreateResponse();
        //}
    }
}
