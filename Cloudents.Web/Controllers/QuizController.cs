using System;
using System.Collections.Generic;
using System.Linq;
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
    public class QuizController : Controller
    {
        private readonly IReadRepositoryAsync<QuizSeoDto, long> _repository;
        private readonly IStringLocalizer<Seo> _localizer;

        public QuizController(IReadRepositoryAsync<QuizSeoDto, long> repository, IStringLocalizer<Seo> localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }

        [Route("quiz/{universityName}/{boxId:long}/{boxName}/{id:long}/{name}", Name = SeoTypeString.Quiz)]
        public async Task<IActionResult> Index(long id, CancellationToken token)
        {
            return this.RedirectToOldSite();
            //var model = await _repository.GetAsync(id, token).ConfigureAwait(false);
            //if (model == null)
            //{
            //    return NotFound();
            //}
            //if (string.IsNullOrEmpty(model.Country)) return View();
            ////SeoBaseUniversityResources.Culture = Languages.GetCultureBaseOnCountry(model.Country);
            //ViewBag.title =
            //    $"{_localizer["QuizTitle"]} - {model.Name} - {model.BoxName} | {_localizer["Cloudents"]}";

            //ViewBag.metaDescription = string.Format(_localizer["QuizMetaDescription"], model.BoxName, model.Name);
            //return View();
        }
    }
}