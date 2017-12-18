using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IStringLocalizer<Seo> _localizer;
        private readonly IReadRepositoryAsync<DocumentSeoDto, long> _repository;

        public DocumentController(IStringLocalizer<Seo> localizer, IReadRepositoryAsync<DocumentSeoDto, long> repository)
        {
            _localizer = localizer;
            _repository = repository;
        }

        [Route("item/{universityName}/{boxId:long}/{boxName}/{id:long}/{name}", Name = SeoTypeString.Item)]
        public async Task<IActionResult> Index(long id, CancellationToken token)
        {
            var model = await _repository.GetAsync(id, token).ConfigureAwait(false);

            if (model == null)
            {
                return NotFound();
            }

            if (!model.Discriminator.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                return View();
            }
            ViewBag.imageSrc = ViewBag.fbImage = "https://az779114.vo.msecnd.net/preview/" + model.ImageUrl +
                                                 ".jpg?width=1200&height=630&mode=crop";
            if (string.IsNullOrEmpty(model.Country)) return View();

            //var culture = Languages.GetCultureBaseOnCountry(model.Country);
            //SeoBaseUniversityResources.Culture = culture;
            //TODO: culture base globalization - localize doesn't work
            ViewBag.title =
                $"{model.BoxName} - {model.DepartmentName} - {model.Name} | {_localizer["Cloudents"]}";

            ViewBag.metaDescription = _localizer["ItemMetaDescription"];
            if (!string.IsNullOrEmpty(model.Description))
            {
                ViewBag.metaDescription += ":" + model.Description.RemoveEndOfString(100);
            }
            ViewBag.metaDescription = WebUtility.HtmlDecode(ViewBag.metaDescription);
            return View();
        }
    }
}