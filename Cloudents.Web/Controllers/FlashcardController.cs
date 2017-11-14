using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Controllers
{
    public class FlashcardController : Controller
    {
        private readonly IStringLocalizer<Seo> _localizer;
        private readonly IReadRepositoryAsync<FlashcardSeoDto, long> _repository;

        public FlashcardController(IStringLocalizer<Seo> localizer, IReadRepositoryAsync<FlashcardSeoDto, long> repository)
        {
            _localizer = localizer;
            _repository = repository;
        }
        //Temp: http://localhost:53216/flashcard/noa-university/100493/tatty/1477/%D7%AA%D7%9E%D7%95%D7%A0%D7%95%D7%AA-%D7%98%D7%95%D7%91%D7%95%D7%AA/
        [Route("flashcard/{universityName}/{boxId:long}/{boxName}/{flashcardId:long}/{flashcardName}", Name = "Flashcard")]
        public async Task<IActionResult> Index(long flashcardId, CancellationToken token)
        {
            ViewBag.fbImage = ViewBag.imageSrc = "/images/3rdParty/fbFlashcard.png";
            var model = await _repository.GetAsync(flashcardId, token).ConfigureAwait(false);

            if (model == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(model.Country)) return View();

            //TODO: need to add specifix culture base on country - culture not working
            //SeoBaseUniversityResources.Culture = Languages.GetCultureBaseOnCountry(model.Country);
            ViewBag.title =
                $"{ _localizer["FlashcardTitle"]} - {model.Name} - {model.BoxName} | {_localizer["Cloudents"]}";

            ViewBag.metaDescription = string.Format(_localizer["FlashcardMetaDescription"], model.Name, model.BoxName);

            return View();
        }
    }
}