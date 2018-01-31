using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class FlashcardController : Controller
    {
        private readonly IReadRepositoryAsync<FlashcardSeoDto, long> _repository;

        public FlashcardController(IReadRepositoryAsync<FlashcardSeoDto, long> repository)
        {
            _repository = repository;
        }

        [Route("flashcard/{universityName}/{boxId:long}/{boxName}/{id:long}/{name}", Name = SeoTypeString.Flashcard)]
        public async Task<IActionResult> IndexAsync(long id, CancellationToken token)
        {
            //return this.RedirectToOldSite();
            ViewBag.fbImage = ViewBag.imageSrc = "/images/3rdParty/fbFlashcard.png";
            var model = await _repository.GetAsync(id, token).ConfigureAwait(false);

            if (model == null)
            {
                return NotFound();
            }
            if (!string.Equals(model.Country, "us", StringComparison.InvariantCultureIgnoreCase))
            {
                return this.RedirectToOldSite();
            }

            if (string.IsNullOrEmpty(model.Country)) return View("Index");

            //TODO: need to add specific culture base on country - culture not working
            //SeoBaseUniversityResources.Culture = Languages.GetCultureBaseOnCountry(model.Country);
            ViewBag.title =
                $"Flashcard - {model.Name} - {model.BoxName} | Spitball";

            ViewBag.metaDescription =
                $"Practice and improve your knowledge in {model.Name} and test yourself with the {model.BoxName} flashcards your classmates have built. Start getting better grades with Spitball!";

            return View("Index");
        }
    }
}