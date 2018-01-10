using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
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
        [Route("flashcard/{universityName}/{boxId:long}/{boxName}/{id:long}/{name}", Name = SeoTypeString.Flashcard)]
        public IActionResult Index(long id, CancellationToken token)
        {
            //return this.RedirectToOldSite();
            ViewBag.fbImage = ViewBag.imageSrc = "/images/3rdParty/fbFlashcard.png";
            var model = await _repository.GetAsync(id, token).ConfigureAwait(false);

            if (model == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(model.Country)) return View();

            //TODO: need to add specific culture base on country - culture not working
            //SeoBaseUniversityResources.Culture = Languages.GetCultureBaseOnCountry(model.Country);
            ViewBag.title =
                $"{ _localizer["FlashcardTitle"]} - {model.Name} - {model.BoxName} | {_localizer["Cloudents"]}";

            ViewBag.metaDescription = string.Format(_localizer["FlashcardMetaDescription"], model.Name, model.BoxName);

            return View();
        }
    }
}