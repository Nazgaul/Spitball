using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Documents;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Schema.NET;

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
        private readonly IUrlBuilder _urlBuilder;



        public DocumentController(
            IDocumentDirectoryBlobProvider blobProvider,
            IStringLocalizer<DocumentController> localizer,
            IQueryBus queryBus, IQueueProvider queueProvider, UserManager<User> userManager, IUrlBuilder urlBuilder)
        {
            _blobProvider = blobProvider;
            _localizer = localizer;
            _queryBus = queryBus;
            _queueProvider = queueProvider;
            _userManager = userManager;
            _urlBuilder = urlBuilder;
        }

        //[Route("item/{universityName}/{boxId:long}/{boxName}/{oldId:long}/{name}")]
        //public async Task<IActionResult> OldDocumentLinkRedirect(string universityName, string boxName, long oldId, string name, CancellationToken token)
        //{
        //    var query = new DocumentSeoByOldId(oldId);
        //    var model = await _queryBus.QueryAsync(query, token);
        //    if (model == null)
        //    {
        //        return NotFound();
        //    }
        //    return RedirectToRoutePermanent(SeoTypeString.Document, new
        //    {
        //        courseName = FriendlyUrlHelper.GetFriendlyTitle(model.CourseName),
        //        id = model.Id,
        //        name = FriendlyUrlHelper.GetFriendlyTitle(model.Name)
        //    });
        //}

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

            _userManager.TryGetLongUserId(User, out var userId);
            var query = new DocumentById(id, userId);
            var model = await _queryBus.QueryAsync(query, token);
            if (model == null)
            {
                return NotFound();
            }
            var t = RedirectToRoutePermanent(SeoTypeString.Document, new
            {
                courseName = FriendlyUrlHelper.GetFriendlyTitle(model.Document.Course),
                id = id.Value,
                name = FriendlyUrlHelper.GetFriendlyTitle(model.Document.Title)
            });
            return t;
        }

        [Route("document/{universityName}/{courseName}/{id:long}/{name}")]
        public async Task<IActionResult> OldDocumentLinkRedirect2(long id, CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new DocumentById(id, userId);

            var model = await _queryBus.QueryAsync(query, token);
            if (model == null)
            {
                return NotFound();
            }
            return RedirectToRoutePermanent(SeoTypeString.Document, new
            {
                courseName = FriendlyUrlHelper.GetFriendlyTitle(model.Document.Course),
                id,
                name = FriendlyUrlHelper.GetFriendlyTitle(model.Document.Title)
            });
        }


        [Route("document/{courseName}/{name}/{id:long}",
             Name = SeoTypeString.Document)]
        [ActionName("Index"), SignInWithToken]
        public async Task<IActionResult> IndexAsync(string courseName, string name, long id, CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new DocumentById(id, userId);

            var model = await _queryBus.QueryAsync(query, token);
            if (model == null)
            {
                return NotFound();
            }

            ViewBag.title = _localizer["Title", model.Document.Course, model.Document.Title];
            ViewBag.metaDescription = _localizer["Description", model.Document.Course];
            if (model.Document.DocumentType == DocumentType.Video && !string.IsNullOrEmpty(model.Document.Snippet))
            {
                var jsonLd = new VideoObject()
                {
                    Description = model.Document.Snippet,
                    Name = model.Document.Title,
                    ThumbnailUrl = new Uri(_urlBuilder.BuildDocumentThumbnailEndpoint(model.Document.Id, new
                    {
                        width = 703,
                        height = 395,
                        mode = "crop"
                    })),
                    UploadDate = model.Document.DateTime,
                    Duration = model.Document.Duration,

                };
                ViewBag.jsonLd = jsonLd;
            }

            return View();
        }

        [Route("document/{courseName}/{name}/{id:long}/download", Name = "ItemDownload")]
        [Authorize]
        public async Task<ActionResult> DownloadAsync(long id, [FromServices] ICommandBus commandBus,
            [FromServices] IBlobProvider blobProvider2, CancellationToken token)
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
            if (item.Document.DocumentType == DocumentType.Video)
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

            Task followTask = Task.CompletedTask;
            //blob.core.windows.net/spitball-files/files/6160/file-82925b5c-e3ba-4f88-962c-db3244eaf2b2-advanced-linux-programming.pdf
            if (item.Document.User.Id != user)
            {
                var command = new FollowUserCommand(item.Document.User.Id, user);
                followTask = commandBus.DispatchAsync(command, token);
            }
            var messageTask = _queueProvider.InsertMessageAsync(new UpdateDocumentNumberOfDownloads(id), token);

            await Task.WhenAll(followTask, messageTask);

            var nameToDownload = item.Document.Title;
            var extension = Path.GetExtension(file);
            var url = blobProvider2.GenerateDownloadLink(uri, TimeSpan.FromMinutes(30), nameToDownload + extension);
            return Redirect(url.AbsoluteUri);
        }
    }
}