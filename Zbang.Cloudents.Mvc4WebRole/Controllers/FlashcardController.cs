using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
//using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{

    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    public class FlashcardController : BaseController
    {
        private readonly IIdGenerator m_IdGenerator;
        private readonly IDocumentDbReadService m_DocumentDbReadService;
        public FlashcardController(IIdGenerator idGenerator, IDocumentDbReadService documentDbReadService)
        {
            m_IdGenerator = idGenerator;
            m_DocumentDbReadService = documentDbReadService;
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
    }
}