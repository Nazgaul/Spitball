using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IStringLocalizer<Seo> _localizer;
        private readonly IReadRepositoryAsync<DocumentSeoDto, long> _repository;
        private readonly IReadRepositoryAsync<DocumentDto, long> _repositoryDocument;
        private readonly IBlobProvider<FilesContainerName> _blobProvider;

        public DocumentController(IStringLocalizer<Seo> localizer, IReadRepositoryAsync<DocumentSeoDto, long> repository, IReadRepositoryAsync<DocumentDto, long> repositoryDocument, IBlobProvider<FilesContainerName> blobProvider)
        {
            _localizer = localizer;
            _repository = repository;
            _repositoryDocument = repositoryDocument;
            _blobProvider = blobProvider;
        }

        [Route("item/{universityName}/{boxId:long}/{boxName}/{id:long}/{name}", Name = SeoTypeString.Item)]
        [ActionName("Index")]
        public async Task<IActionResult> IndexAsync(long id, CancellationToken token)
        {
            //return this.RedirectToOldSite();


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
            //if (string.IsNullOrEmpty(model.Country)) return View();

            //var culture = Languages.GetCultureBaseOnCountry(model.Country);
            //SeoBaseUniversityResources.Culture = culture;
            //TODO: culture base globalization - localize doesn't work
            ViewBag.title =
                $"{model.BoxName} - {model.Name} | {_localizer["Cloudents"]}";

            ViewBag.metaDescription = _localizer["ItemMetaDescription"];
            if (!string.IsNullOrEmpty(model.Description))
            {
                ViewBag.metaDescription += ":" + model.Description.RemoveEndOfString(100);
            }
            ViewBag.metaDescription = WebUtility.HtmlDecode(ViewBag.metaDescription);
            return View();
        }


        [Route("Item/{universityName}/{boxId:long}/{boxName}/{itemid:long:min(0)}/{itemName}/download", Name = "ItemDownload")]
        [Route("D/{boxId:long:min(0)}/{itemId:long:min(0)}", Name = "ItemDownload2")]
        public async Task<ActionResult> DownloadAsync(long itemId, CancellationToken token)
        {

            var item = await _repositoryDocument.GetAsync(itemId, token);
            if (item.Type == "Link")
            {
                return Redirect(item.Blob);
            }

            var nameToDownload = Path.GetFileNameWithoutExtension(item.Name);
            var extension = Path.GetExtension(item.Blob);
            var url = _blobProvider.GenerateSharedAccessReadPermission(item.Blob, 30, nameToDownload + extension);
            return Redirect(url);
        }
    }
}