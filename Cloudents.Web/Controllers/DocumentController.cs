using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Web.Binders;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DocumentController : Controller
    {
        private readonly IBlobProvider<OldSbFilesContainerName> _blobProvider;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IStringLocalizer<DocumentController> _localizer;
        private readonly IQueryBus _queryBus;

        public DocumentController(
            IBlobProvider<OldSbFilesContainerName> blobProvider, IStringLocalizer<SharedResource> sharedLocalizer,
            IStringLocalizer<DocumentController> localizer, IQueryBus queryBus)
        {
            _blobProvider = blobProvider;
            _sharedLocalizer = sharedLocalizer;
            _localizer = localizer;
            _queryBus = queryBus;
        }

        //[Route("item/{universityName}/{boxId:long}/{boxName}/{id:long}/{name}", Name = SeoTypeString.Item)]
        //public IActionResult OldDocumentLinkRedirect()
        //{

        //}

        [Route("document/{universityName}/{boxId}/{id:long}/{name}", Name = "Document")]
        [ActionName("Index")]
        public async Task<IActionResult> IndexAsync(long id,
            [ModelBinder(typeof(CountryModelBinder))] string country,
            CancellationToken token)
        {
            var query = new DocumentById(id);
            var model = await _queryBus.QueryAsync<DocumentSeoDto>(query, token);
            if (model == null)
            {
                return NotFound();
            }

            //if (!model.Discriminator.Equals("file", StringComparison.OrdinalIgnoreCase))
            //{
            //    return View();
            //}
            //ViewBag.imageSrc = ViewBag.fbImage = "https://az779114.vo.msecnd.net/preview/" + model.ImageUrl +
            //                                     ".jpg?width=1200&height=630&mode=crop";
            ViewBag.country = country ?? "us";
            if (string.IsNullOrEmpty(model.Country)) return View();
            
            
            //TODO: need to be university culture
            ViewBag.title =
                $"{model.CourseName} - {model.Name} | {_sharedLocalizer["Spitball"]}";

            ViewBag.metaDescription = _localizer["meta"];
            if (!string.IsNullOrEmpty(model.Description))
            {
                ViewBag.metaDescription += ":" + model.Description.Truncate(100);
            }
            ViewBag.metaDescription = WebUtility.HtmlDecode(ViewBag.metaDescription);
            return View();
        }

        [Route("Document/{id:long:min(0)}/{itemName}/download", Name = "ItemDownload")]
        [Route("D/{id:long:min(0)}", Name = "ItemDownload2")]
        public async Task<ActionResult> DownloadAsync(long id, CancellationToken token)
        {
            var query = new DocumentById(id);
            var item = await _queryBus.QueryAsync<DocumentDetailDto>(query, token);
            if (item == null)
            {
                return NotFound();
            }
            

            var nameToDownload = Path.GetFileNameWithoutExtension(item.Name);
            var extension = Path.GetExtension(item.Blob);
            var url = _blobProvider.GenerateSharedAccessReadPermission(item.Blob, 30, nameToDownload + extension);
            return Redirect(url);
        }
    }
}