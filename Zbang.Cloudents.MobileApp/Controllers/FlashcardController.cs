using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
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
    public class FlashcardController : ApiController
    {
        private readonly IQueueProvider m_QueueProvider;
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IDocumentDbReadService m_DocumentDbReadService;
        private readonly IIdGenerator m_IdGenerator;
        //private readonly IZboxReadSecurityReadService m_ZboxReadSecurityService;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly Lazy<IBlobProvider> m_BlobProvider;
        private readonly IBlobProvider2<FlashcardContainerName> m_FlashcardBlob;

        public FlashcardController(IQueueProvider queueProvider, IZboxCacheReadService zboxReadService, IDocumentDbReadService documentDbReadService, IZboxWriteService zboxWriteService, IBlobProvider2<FlashcardContainerName> flashcardBlob, IIdGenerator idGenerator, Lazy<IBlobProvider> blobProvider)
        {
            m_QueueProvider = queueProvider;
            m_ZboxReadService = zboxReadService;
            m_DocumentDbReadService = documentDbReadService;
            m_ZboxWriteService = zboxWriteService;
            m_FlashcardBlob = flashcardBlob;
            m_IdGenerator = idGenerator;
            m_BlobProvider = blobProvider;
        }

        // GET api/Flashcard
        public async Task<HttpResponseMessage> Get(long id)
        {
            var userId = User.GetUserId(false);
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
            await Task.WhenAll(tTransAction, tValues, tUserValues);
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
                //values.UserId,
                tUserValues.Result.Pins,
                tUserValues.Result.Like,
                //tUserValues.Result.OwnerName
                //tUserValues.Result.UniversityData

            });
            //return "Hello from custom controller!";
        }

        [HttpPost, Route("api/flashcard/{id:long}/pin")]
        public HttpResponseMessage Pin([FromUri] long id, [FromBody] PinRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var command = new AddFlashcardPinCommand(User.GetUserId(), id, model.Index);
            m_ZboxWriteService.AddPinFlashcard(command);
            return Request.CreateResponse(new { id, model });
        }

        [HttpDelete, Route("api/flashcard/{id:long}/pin")]
        public HttpResponseMessage DeletePin(long id, int index)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var command = new DeleteFlashcardPinCommand(User.GetUserId(), id, index);
            m_ZboxWriteService.DeletePinFlashcard(command);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpPost, Route("api/flashcard/{id:long}/like")]
        public async Task<HttpResponseMessage> AddLikeAsync(long id)
        {
            var command = new AddFlashcardLikeCommand(User.GetUserId(), id);
            await m_ZboxWriteService.AddFlashcardLikeAsync(command);
            return Request.CreateResponse(HttpStatusCode.OK, command.Id);
        }
        [HttpDelete, Route("api/flashcard/{id:long}/like")]
        public async Task<HttpResponseMessage> DeleteLikeAsync(long id, Guid likeId)
        {
            var command = new DeleteFlashcardLikeCommand(User.GetUserId(), likeId);
            await m_ZboxWriteService.DeleteFlashcardLikeAsync(command);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }


        [HttpGet]
        [Route("api/flashcard/image")]
        public string UploadImage(string blob, string mimeType)
        {
            return m_FlashcardBlob.GenerateSharedAccessWritePermission(blob, mimeType);
        }


        [HttpDelete, Route("api/flashcard/image")]
        public async Task<HttpResponseMessage> FlashcardImageRemoveAsync(Uri image, CancellationToken cancellationToken)
        {

            //var values = await m_DocumentDbReadService.FlashcardAsync(id);
            //if (values.Publish)
            //{
            //    throw new ArgumentException("Flashcard is published");
            //}
            //if (values.UserId != User.GetUserId())
            //{
            //    throw new ArgumentException("This is not the owner");
            //}
            var blobName = m_BlobProvider.Value.GetBlobNameFromUri(image);
            await m_FlashcardBlob.RemoveBlobAsync(blobName, cancellationToken);
            return Request.CreateResponse(string.Empty);

        }

        [HttpPost, Route("api/flashcard/publish")]
        public async Task<HttpResponseMessage> PublishAsync(
            FlashcardRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            if (model.Cards != null)
            {
                for (var i = model.Cards.Count - 1; i >= 0; --i)
                {
                    if (model.Cards[i].IsEmpty())
                    {
                        model.Cards.RemoveAt(i);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            var id = m_IdGenerator.GetId(IdContainer.FlashcardScope);
            var flashCard = new Zbox.Domain.Flashcard(id)
            {
                BoxId = model.BoxId,
                UserId = User.GetUserId(),
                Name = model.Name,
                Publish = false,
                DateTime = DateTime.UtcNow,
                Cards = model.Cards?.Select(s => new Zbox.Domain.Card
                {
                    Cover = new Zbox.Domain.CardSlide
                    {
                        Text = s?.Cover?.Text,
                        Image = s?.Cover?.Image
                    },
                    Front = new Zbox.Domain.CardSlide
                    {
                        Image = s?.Front?.Image,
                        Text = s?.Front?.Text
                    }
                })
            };
            try
            {
                var commandCreate = new AddFlashcardCommand(flashCard);
                await m_ZboxWriteService.AddFlashcardAsync(commandCreate);

                flashCard.Publish = true;

                var command = new PublishFlashcardCommand(flashCard, model.BoxId);
                await m_ZboxWriteService.PublishFlashcardAsync(command);
                return Request.CreateResponse(id);
            }
            catch (ArgumentException)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
                //return JsonError("flashcard with the same name already exists");
            }
        }
    }
}
