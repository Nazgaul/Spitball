using System;
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
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
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
        private readonly Lazy<IBlobProvider> m_BlobProvider;
        public FlashcardController(IIdGenerator idGenerator, IDocumentDbReadService documentDbReadService, IBlobProvider2<FlashcardContainerName> flashcardBlob, Lazy<IBlobProvider> blobProvider)
        {
            m_IdGenerator = idGenerator;
            m_DocumentDbReadService = documentDbReadService;
            m_FlashcardBlob = flashcardBlob;
            m_BlobProvider = blobProvider;
        }

        // GET: Flashcard
        //public ActionResult Index()
        //{
        //    return View();
        //}

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
            //var query = new GetQuizDraftQuery(quizId);
            var values = await m_DocumentDbReadService.FlashcardAsync(id);
            if (values.Publish)
            {
                throw new ArgumentException("Flashcard is published");
            }
            if (values.UserId != User.GetUserId())
            {
                throw new ArgumentException("This is not the owner");
            }
            return JsonOk(new
            {
                values.Cards,
                values.Name

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
                Cards = model.Cards.Select(s => new Zbox.Domain.Card
                {
                    Cover = new Zbox.Domain.CardSlide
                    {
                        Text = s.Cover.Text,
                        Image = s.Cover.Image
                    },
                    Front = new Zbox.Domain.CardSlide
                    {
                        Image = s.Front.Image,
                        Text = s.Front.Text
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

            //var url = await m_BlobProvider.UploadQuizImageAsync(file.InputStream, file.ContentType, boxId, file.FileName);
            //return JsonOk(url);
        }
    }
}