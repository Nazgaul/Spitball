using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{

    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    public class FlashcardController : BaseController
    {
        private readonly IIdGenerator m_IdGenerator;
        private readonly IDocumentDbReadService m_DocumentDbReadService;
        private readonly IBlobProvider2<FlashcardContainerName> m_FlashcardBlob;
        private readonly IQueueProvider m_QueueProvider;
        private readonly Lazy<IBlobProvider> m_BlobProvider;
        public FlashcardController(IIdGenerator idGenerator, IDocumentDbReadService documentDbReadService, IBlobProvider2<FlashcardContainerName> flashcardBlob, Lazy<IBlobProvider> blobProvider, IQueueProvider queueProvider)
        {
            m_IdGenerator = idGenerator;
            m_DocumentDbReadService = documentDbReadService;
            m_FlashcardBlob = flashcardBlob;
            m_BlobProvider = blobProvider;
            m_QueueProvider = queueProvider;
        }

        [Route("flashcard/{universityName}/{boxId:long}/{boxName}/{itemid:long}/{itemName}", Name = "Flashcard")]
        public ActionResult Index()
        {
            return View("Empty");
        }


        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult IndexPartial()
        {
            return PartialView("Index");
        }

        [ZboxAuthorize]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult CreatePartial()
        {
            return PartialView();
        }

        [HttpGet]
        [ZboxAuthorize, ActionName("Draft")]
        public async Task<ActionResult> DraftAsync(long id)
        {
            var values = await m_DocumentDbReadService.FlashcardAsync(id);
            if (values.UserId != User.GetUserId())
            {
                throw new ArgumentException("This is not the owner");
            }
            if (values.IsDeleted)
            {
                throw new ArgumentException("Flashcard is deleted");
            }
            return JsonOk(new
            {
                values.Cards,
                values.Name

            });
        }
        [ActionName("Data")]
        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("boxId")]
        public async Task<ActionResult> DataAsync(long id, long boxId)
        {
            var tTransAction = m_QueueProvider.InsertMessageToTranactionAsync(
                      new StatisticsData4(new List<StatisticsData4.StatisticItemData>
                    {
                        new StatisticsData4.StatisticItemData
                        {
                            Id = id,
                            Action = (int)StatisticsAction.Flashcard
                        }
                    }));
            var tUserValues =
                ZboxReadService.GetUserFlashcardAsync(new Zbox.ViewModel.Queries.GetUserFlashcardQuery(
                    User.GetUserId(), id));
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
            return JsonOk(new
            {
                values.Cards,
                values.Name,
                values.UserId,
                tUserValues.Result.Pins,
                tUserValues.Result.Like

            });
        }

        [HttpPost, ZboxAuthorize, ActionName("Index")]
        public async Task<JsonResult> CreateAsync(Flashcard model, long boxId)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var id = m_IdGenerator.GetId(IdContainer.FlashcardScope);
            var flashCard = new Zbox.Domain.Flashcard(id)
            {
                BoxId = boxId,
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

            var command = new AddFlashcardCommand(flashCard);
            await ZboxWriteService.AddFlashcardAsync(command);
            return JsonOk(id);
        }

        [HttpPost, ZboxAuthorize, ActionName("publish")]
        public async Task<JsonResult> PublishAsync(long id, Flashcard model, long boxId)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var flashCard = new Zbox.Domain.Flashcard(id)
            {
                BoxId = boxId,
                UserId = User.GetUserId(),
                Name = model.Name,
                Publish = true,
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

            var command = new PublishFlashcardCommand(flashCard);
            await ZboxWriteService.PublishFlashcardAsync(command);
            return JsonOk();
        }
        [HttpPut, ZboxAuthorize, ActionName("Index")]
        public async Task<JsonResult> UpdateAsync(long id, Flashcard model, long boxId)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var flashCard = new Zbox.Domain.Flashcard(id)
            {
                BoxId = boxId,
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

            var command = new UpdateFlashcardCommand(flashCard);
            await ZboxWriteService.UpdateFlashcardAsync(command);
            return JsonOk();
        }

        [HttpDelete, ZboxAuthorize, ActionName("Image")]
        public async Task<JsonResult> FlashcardImageRemoveAsync(long id, Uri image, CancellationToken cancellationToken)
        {
            using (var source = CreateCancellationToken(cancellationToken))
            {
                var values = await m_DocumentDbReadService.FlashcardAsync(id);
                if (values.Publish)
                {
                    throw new ArgumentException("Flashcard is published");
                }
                if (values.UserId != User.GetUserId())
                {
                    throw new ArgumentException("This is not the owner");
                }
                var blobName = m_BlobProvider.Value.GetBlobNameFromUri(image);
                await m_FlashcardBlob.RemoveBlobAsync(blobName, source.Token);
                return JsonOk();
            }
        }
        [HttpDelete, ZboxAuthorize, ActionName("Index")]
        public async Task<JsonResult> DeleteAsync(long id)
        {
            var command = new DeleteFlashcardCommand(id, User.GetUserId());
            await ZboxWriteService.DeleteFlashcardAsync(command);
            //var values = await m_DocumentDbReadService.FlashcardAsync(id);
            //if (values.Publish)
            //{
            //    throw new ArgumentException("Flashcard is published");
            //}
            //if (values.UserId != User.GetUserId())
            //{
            //    throw new ArgumentException("This is not the owner");
            //}
            //var images = values.Cards.Select(s => s?.Cover?.Image).Union(values.Cards.Select(s => s?.Front?.Image));
            //var tasks = new List<Task>();
            //foreach (var image in images)
            //{

            //    var blobName = m_BlobProvider.Value.GetBlobNameFromUri(new Uri(image));
            //    tasks.Add(m_FlashcardBlob.RemoveBlobAsync(blobName, default(CancellationToken)));
            //}

            //await Task.WhenAll(tasks);

            return JsonOk();
        }

        [HttpPost, ZboxAuthorize, ActionName("Image")]
        public async Task<JsonResult> FlashcardImageAsync(CancellationToken cancellationToken)
        {
            var file = HttpContext.Request.Files?[0];
            if (file == null)
            {
                return JsonError("No files");
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            using (var source = CreateCancellationToken(cancellationToken))
            {
                await m_FlashcardBlob.UploadStreamAsync(fileName, file.InputStream, file.ContentType, source.Token);
                var url = m_FlashcardBlob.GetBlobUrl(fileName, true);
                return JsonOk(url);
            }

        }

        [HttpPost, ZboxAuthorize]
        public JsonResult Pin(Pin model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var command = new AddFlashcardPinCommand(User.GetUserId(), model.Id.GetValueOrDefault(), model.Index.GetValueOrDefault());
            ZboxWriteService.AddPinFlashcard(command);
            return JsonOk();
        }

        [HttpDelete, ZboxAuthorize,ActionName("Pin")]
        public JsonResult DeletePin(Pin model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var command = new DeleteFlashcardPinCommand(User.GetUserId(), model.Id.GetValueOrDefault(), model.Index.GetValueOrDefault());
            ZboxWriteService.DeletePinFlashcard(command);
            return JsonOk();
        }

        [HttpPost, ZboxAuthorize,ActionName("like")]
        public JsonResult AddLike(long id)
        {
            var command = new AddFlashcardLikeCommand(User.GetUserId(), id);
            ZboxWriteService.AddFlashcardLike(command);
            return JsonOk(command.Id);
        }
        [HttpDelete, ZboxAuthorize, ActionName("like")]
        public JsonResult DeleteLike(Guid id)
        {
            var command = new DeleteFlashcardLikeCommand(User.GetUserId(), id);
            ZboxWriteService.DeleteFlashcardLike(command);
            return JsonOk();
        }
    }
}