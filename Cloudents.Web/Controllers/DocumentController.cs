using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Resources;
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
        private readonly IQueueProvider _queueProvider;



        public DocumentController(
            IBlobProvider<DocumentContainer> blobProvider,
            IStringLocalizer<SharedResource> sharedLocalizer,
            IStringLocalizer<DocumentController> localizer,
            IQueryBus queryBus, IQueueProvider queueProvider)
        {
            _blobProvider = blobProvider;
            _sharedLocalizer = sharedLocalizer;
            _localizer = localizer;
            _queryBus = queryBus;
            _queueProvider = queueProvider;
        }

        [Route("item/{universityName}/{boxId:long}/{boxName}/{id:long}/{name}", Name = SeoTypeString.Item)]
        public IActionResult OldDocumentLinkRedirect(string universityName, string boxName, long id, string name)
        {
            //return this.RedirectToOldSite();
            //TODO: we need to put Permanent
            return RedirectToAction("Index", new
            {
                universityName,
                courseName = boxName,
                id,
                name
            });
        }

        [Route("document/{base62}",Name = "ShortDocumentLink")]
        public async Task<IActionResult> ShortUrl(string base62,
            CancellationToken token)
        {
            if (string.IsNullOrEmpty(base62))
            {
                return NotFound();
            }
            if (!Base62.TryParse(base62, out var id))
            {
                return NotFound();
            }
            var query = new DocumentById(id);
            var model = await _queryBus.QueryAsync<DocumentSeoDto>(query, token);
            if (model == null)
            {
                return NotFound();
            }
            return RedirectToRoutePermanent(SeoTypeString.Document, new
            {
                universityName = FriendlyUrlHelper.GetFriendlyTitle(model.UniversityName),
                courseName = FriendlyUrlHelper.GetFriendlyTitle(model.CourseName),
                id,
                name = FriendlyUrlHelper.GetFriendlyTitle(model.Name)
            });
        }

        [Route("document/{universityName}/{courseName}/{id:long}/{name}", Name = SeoTypeString.Document)]
        [ActionName("Index")]
        public async Task<IActionResult> IndexAsync(long id, string courseName, string name, string universityName,
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
            //var metaContent = await _documentSearch.ItemMetaContentAsync(id, token);
            if (string.IsNullOrEmpty(model.Country)) return View();

            //TODO: need to be university culture
            ViewBag.title =
                $"{model.CourseName} - {model.Name} | {_sharedLocalizer["Spitball"]}";

            ViewBag.metaDescription = _localizer["meta"];
            if (!string.IsNullOrEmpty(model.MetaContent))
            {
                ViewBag.metaDescription += ":" + model.MetaContent.Truncate(100);
            }
            ViewBag.metaDescription = WebUtility.HtmlDecode(ViewBag.metaDescription);
            return View();
        }

        [Route("document/{universityName}/{courseName}/{id:long}/{name}/download", Name = "ItemDownload")]
        [Authorize]
        public async Task<ActionResult> DownloadAsync(long id, [FromServices] IBlobProvider blobProvider2, CancellationToken token)
        {
            
            var query = new DocumentById(id);
            var tItem = _queryBus.QueryAsync<DocumentDetailDto>(query, token);
            var tFiles = _blobProvider.FilesInDirectoryAsync("file-", id.ToString(), token);
            await Task.WhenAll(tItem, tFiles);

            var item = tItem.Result;
            if (item == null)
            {
                return NotFound();
            }

            var files = tFiles.Result;
            var uri = files.First();
            var file = uri.Segments.Last();

            //blob.core.windows.net/spitball-files/files/6160/file-82925b5c-e3ba-4f88-962c-db3244eaf2b2-advanced-linux-programming.pdf

            await _queueProvider.InsertMessageAsync(new UpdateDocumentNumberOfDownloads(id), token);
            var nameToDownload = Path.GetFileNameWithoutExtension(item.Name);
            var extension = Path.GetExtension(file);
            var url = blobProvider2.GenerateDownloadLink(uri, 30, nameToDownload + extension);
            return Redirect(url.AbsoluteUri);
        }
    }
}