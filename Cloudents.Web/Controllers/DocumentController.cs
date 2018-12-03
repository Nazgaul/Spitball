using System;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Message.System;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DocumentController : Controller
    {
        private readonly IBlobProvider<DocumentContainer> _blobProvider;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IStringLocalizer<DocumentController> _localizer;
        private readonly IQueryBus _queryBus;
        private readonly IDocumentSearch _documentSearch;
        private readonly IQueueProvider _queueProvider;



        public DocumentController(
            IBlobProvider<DocumentContainer> blobProvider,
            IStringLocalizer<SharedResource> sharedLocalizer,
            IStringLocalizer<DocumentController> localizer,
            IQueryBus queryBus, IDocumentSearch documentSearch)
        {
            _blobProvider = blobProvider;
            _sharedLocalizer = sharedLocalizer;
            _localizer = localizer;
            _queryBus = queryBus;
            _documentSearch = documentSearch;
        }

        [Route("item/{universityName}/{boxId:long}/{boxName}/{id:long}/{name}", Name = SeoTypeString.Item)]
        public IActionResult OldDocumentLinkRedirect(string universityName, string boxName, long id, string name)
        {
            return this.RedirectToOldSite();
            //TODO: we need to put Permanent
            //return RedirectToAction("Index", new
            //{
            //    universityName,
            //    boxId = boxName,
            //    id,
            //    name
            //});
        }

        [Route("document/{universityName}/{courseName}/{id:long}/{name}", Name = SeoTypeString.Document)]
        [ActionName("Index")]
        public async Task<IActionResult> IndexAsync(long id, string courseName, string name, string universityName,
            //[ModelBinder(typeof(CountryModelBinder))] string country,
            CancellationToken token)
        {
            var query = new DocumentById(id);

            var model = await _queryBus.QueryAsync<DocumentSeoDto>(query, token);
            if (model == null)
            {
                return NotFound();
            }


            var compareCourseResult = FriendlyUrlHelper.CompareTitle(model.CourseName, courseName);
            var compareNameResult = FriendlyUrlHelper.CompareTitle(model.Name, name);

            if (compareCourseResult == FriendlyUrlHelper.TitleCompareResult.NotEqual ||
                compareNameResult == FriendlyUrlHelper.TitleCompareResult.NotEqual)
            {
                return NotFound();
            }

            if (compareNameResult ==FriendlyUrlHelper.TitleCompareResult.EqualNotFriendly ||
                compareNameResult == FriendlyUrlHelper.TitleCompareResult.EqualNotFriendly)
            {
                return RedirectToRoutePermanent(SeoTypeString.Document, new
                {
                    universityName = FriendlyUrlHelper.GetFriendlyTitle(model.UniversityName),
                    courseName = FriendlyUrlHelper.GetFriendlyTitle(model.CourseName),
                    id,
                    name = FriendlyUrlHelper.GetFriendlyTitle(model.Name)
                });

            }

            if (!FriendlyUrlHelper.GetFriendlyTitle(model.CourseName).Equals(courseName, StringComparison.OrdinalIgnoreCase)
            || !FriendlyUrlHelper.GetFriendlyTitle(model.Name).Equals(name, StringComparison.OrdinalIgnoreCase)
                )
            {
                return NotFound();
            }
            var metaContent = await _documentSearch.ItemMetaContentAsync(id, token);
            if (string.IsNullOrEmpty(model.Country)) return View();

            //TODO: need to be university culture
            ViewBag.title =
                $"{model.CourseName} - {model.Name} | {_sharedLocalizer["Spitball"]}";

            ViewBag.metaDescription = _localizer["meta"];
            if (!string.IsNullOrEmpty(metaContent))
            {
                ViewBag.metaDescription += ":" + metaContent.Truncate(100);
            }
            ViewBag.metaDescription = WebUtility.HtmlDecode(ViewBag.metaDescription);
            return View();
        }

        [Route("document/{universityName}/{courseName}/{id:long}/{name}/download", Name = "ItemDownload")]
        [Authorize]
        public async Task<ActionResult> DownloadAsync(long id, CancellationToken token)
        {
            var query = new DocumentById(id);
            var item = await _queryBus.QueryAsync<DocumentDetailDto>(query, token);
            if (item == null)
            {
                return NotFound();
            }

            await _queueProvider.InsertMessageAsync(new UpdateDocumentNumberOfDownloads(id), token);
            var nameToDownload = Path.GetFileNameWithoutExtension(item.Name);
            var extension = Path.GetExtension(item.Blob);
            var url = _blobProvider.GenerateDownloadLink($"{id}/{item.Blob}", 30, nameToDownload + extension);
            return Redirect(url);
        }
    }
}