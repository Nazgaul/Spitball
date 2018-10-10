﻿using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DocumentController : Controller
    {
        private readonly IReadRepositoryAsync<DocumentSeoDto, long> _repository;
        private readonly IReadRepositoryAsync<DocumentDto, long> _repositoryDocument;
        private readonly IBlobProvider<FilesContainerName> _blobProvider;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IStringLocalizer<DocumentController> _localizer;

        public DocumentController(
            IReadRepositoryAsync<DocumentSeoDto, long> repository,
            IReadRepositoryAsync<DocumentDto, long> repositoryDocument,
            IBlobProvider<FilesContainerName> blobProvider, IStringLocalizer<SharedResource> sharedLocalizer,
            IStringLocalizer<DocumentController> localizer)
        {
            _repository = repository;
            _repositoryDocument = repositoryDocument;
            _blobProvider = blobProvider;
            _sharedLocalizer = sharedLocalizer;
            _localizer = localizer;
        }

        [Route("item/{universityName}/{boxId:long}/{boxName}/{id:long}/{name}", Name = SeoTypeString.Item)]
        [ActionName("Index")]
        public async Task<IActionResult> IndexAsync(long id, CancellationToken token)
        {
            var model = await _repository.GetAsync(id, token).ConfigureAwait(false);
            if (model == null)
            {
                return NotFound();
            }

            //if (string.Equals(model.Country, "il", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return this.RedirectToOldSite();
            //}

            if (!model.Discriminator.Equals("file", StringComparison.OrdinalIgnoreCase))
            {
                return View();
            }
            ViewBag.imageSrc = ViewBag.fbImage = "https://az779114.vo.msecnd.net/preview/" + model.ImageUrl +
                                                 ".jpg?width=1200&height=630&mode=crop";
            if (string.IsNullOrEmpty(model.Country)) return View();

            
            //var culture = Languages.GetCultureBaseOnCountry(model.Country);
            //_localizer.WithCulture()
            //SeoBaseUniversityResources.Culture = culture;
            //TODO: culture base globalization - localize doesn't work
            ViewBag.title =
                $"{model.BoxName} - {model.Name} | {_sharedLocalizer["Spitball"]}";

            ViewBag.metaDescription = _localizer["meta"];
            if (!string.IsNullOrEmpty(model.Description))
            {
                ViewBag.metaDescription += ":" + model.Description.Truncate(100);
            }
            ViewBag.metaDescription = WebUtility.HtmlDecode(ViewBag.metaDescription);
            return View();
        }

        [Route("Item/{universityName}/{boxId:long}/{boxName}/{itemid:long:min(0)}/{itemName}/download", Name = "ItemDownload")]
        [Route("D/{boxId:long:min(0)}/{itemId:long:min(0)}", Name = "ItemDownload2")]
        public async Task<ActionResult> DownloadAsync(long itemId, CancellationToken token)
        {
            var item = await _repositoryDocument.GetAsync(itemId, token).ConfigureAwait(false);
            if (item == null)
            {
                return NotFound();
            }
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