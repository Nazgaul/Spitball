using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Web.Filters;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DocumentController : Controller
    {
        private readonly IDocumentDirectoryBlobProvider _blobProvider;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<DocumentController> _localizer;
        private readonly IQueryBus _queryBus;
        private readonly IQueueProvider _queueProvider;



        public DocumentController(
            IDocumentDirectoryBlobProvider blobProvider,
            IStringLocalizer<DocumentController> localizer,
            IQueryBus queryBus, IQueueProvider queueProvider, UserManager<User> userManager)
        {
            _blobProvider = blobProvider;
            _localizer = localizer;
            _queryBus = queryBus;
            _queueProvider = queueProvider;
            _userManager = userManager;
        }

        [Route("item/{universityName}/{boxId:long}/{boxName}/{oldId:long}/{name}")]
        public async Task<IActionResult> OldDocumentLinkRedirect(string universityName, string boxName, long oldId, string name, CancellationToken token)
        {
            var query = new DocumentSeoByOldId(oldId);
            var model = await _queryBus.QueryAsync(query, token);
            if (model == null)
            {
                return NotFound();
            }
            return RedirectToRoutePermanent(SeoTypeString.Document, new
            {
                courseName = FriendlyUrlHelper.GetFriendlyTitle(model.CourseName),
                id = model.Id,
                name = FriendlyUrlHelper.GetFriendlyTitle(model.Name)
            });
        }

        [Route("document/{base62}", Name = "ShortDocumentLink")]
        public async Task<IActionResult> ShortUrl(string base62,
            CancellationToken token)
        {
            if (string.IsNullOrEmpty(base62))
            {
                return NotFound();
            }

            //if (!long.TryParse(base62, out var id))
            //{
            if (!Base62.TryParse(base62, out var id))
            {
                return NotFound();
            }
           

            var query = new DocumentSeoById(id);
            var model = await _queryBus.QueryAsync(query, token);
            if (model == null)
            {
                return NotFound();
            }
            var t = RedirectToRoutePermanent(SeoTypeString.Document, new
            {
                courseName = FriendlyUrlHelper.GetFriendlyTitle(model.CourseName),
                id = id.Value,
                name = FriendlyUrlHelper.GetFriendlyTitle(model.Name)
            });
            return t;
        }

        [Route("document/{universityName}/{courseName}/{id:long}/{name}")]
        public async Task<IActionResult> OldDocumentLinkRedirect2(long id, CancellationToken token)
        {
            var query = new DocumentSeoById(id);

            var model = await _queryBus.QueryAsync(query, token);
            if (model == null)
            {
                return NotFound();
            }
            return RedirectToRoutePermanent(SeoTypeString.Document, new
            {
                courseName = FriendlyUrlHelper.GetFriendlyTitle(model.CourseName),
                id,
                name = FriendlyUrlHelper.GetFriendlyTitle(model.Name)
            });
        }


       [Route("document/{courseName}/{name}/{id:long}", 
            Name = SeoTypeString.Document)]
        [ActionName("Index"), SignInWithToken]
        public async Task<IActionResult> IndexAsync(string courseName, string name, long id, CancellationToken token)
        {
            var query = new DocumentSeoById(id);

            var model = await _queryBus.QueryAsync(query, token);
            if (model == null)
            {
                return NotFound();
            }

            ViewBag.title = _localizer["Title", model.CourseName, model.Name];
            ViewBag.metaDescription = _localizer["Description",model.CourseName,model.UniversityName];
            return View();
        }

        [Route("document/{courseName}/{name}/{id:long}/download", Name = "ItemDownload")]
        [Authorize]
        public async Task<ActionResult> DownloadAsync(long id, [FromServices] IBlobProvider blobProvider2, CancellationToken token)
        {
            var user = _userManager.GetLongUserId(User);
            var query = new DocumentById(id, user);
            var tItem = _queryBus.QueryAsync(query, token);
            var tFiles = _blobProvider.FilesInDirectoryAsync("file-", id.ToString(), token);
            await Task.WhenAll(tItem, tFiles);

            var item = tItem.Result;
           
            if (item == null)
            {
                return NotFound();
            }
            if (item.DocumentType == DocumentType.Video)
            {
                return Unauthorized();
            }
            if (!item.IsPurchased)
            {
                return Unauthorized();
            }

            var files = tFiles.Result;
            var uri = files.First();
            var file = uri.Segments.Last();

            //blob.core.windows.net/spitball-files/files/6160/file-82925b5c-e3ba-4f88-962c-db3244eaf2b2-advanced-linux-programming.pdf

            await _queueProvider.InsertMessageAsync(new UpdateDocumentNumberOfDownloads(id), token);
            var nameToDownload = Path.GetFileNameWithoutExtension(item.Name);
            var extension = Path.GetExtension(file);
            var url = blobProvider2.GenerateDownloadLink(uri, TimeSpan.FromMinutes(30), nameToDownload + extension);
            return Redirect(url.AbsoluteUri);
        }
    }
}