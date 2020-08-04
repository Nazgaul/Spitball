using Cloudents.Command;
using Cloudents.Command.Command;
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
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DocumentController : Controller
    {
        private readonly IDocumentDirectoryBlobProvider _blobProvider;
        private readonly UserManager<User> _userManager;
        private readonly IQueryBus _queryBus;
        private readonly IQueueProvider _queueProvider;



        public DocumentController(
            IDocumentDirectoryBlobProvider blobProvider,
            IQueryBus queryBus, IQueueProvider queueProvider, UserManager<User> userManager)
        {
            _blobProvider = blobProvider;
            _queryBus = queryBus;
            _queueProvider = queueProvider;
            _userManager = userManager;
        }

        [Route("d/{id}", Name = "ShortDocumentLink2")]
        public async Task<IActionResult> ShortUrl2Async(long id, string theme,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new DocumentById(id, userId);
            var model = await _queryBus.QueryAsync(query, token);
            if (model == null)
            {
                return NotFound();
            }

            return RedirectToRoute(SeoTypeString.Tutor, new
            {
                id = model.UserId,
                name = FriendlyUrlHelper.GetFriendlyTitle(model.UserName),
                d = id
            });
        }
        

        [Route("document/{universityName}/{courseName}/{id:long}/{name}")]
        public async Task<IActionResult> OldDocumentLinkRedirect2Async(long id, CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new DocumentById(id, userId);

            var model = await _queryBus.QueryAsync(query, token);
            if (model == null)
            {
                return NotFound();
            }

            return RedirectToRoute(SeoTypeString.Tutor, new
            {
                id = model.UserId,
                name = FriendlyUrlHelper.GetFriendlyTitle(model.UserName),
                d = id
            });


        }


        [Route("document/{courseName}/{name}/{id:long}",
             Name = SeoTypeString.Document)]
        [ActionName("Index"), SignInWithToken]
        public async Task<IActionResult> IndexAsync([FromQuery] string theme, long id, CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new DocumentById(id, userId);

            var model = await _queryBus.QueryAsync(query, token);
            if (model == null)
            {
                return NotFound();
            }


            return RedirectToRoute(SeoTypeString.Tutor, new
            {
                id = model.UserId,
                name = FriendlyUrlHelper.GetFriendlyTitle(model.UserName),
                d = id
            });




        }

        [Route("document/{id:long}/download", Name = "ItemDownload")]
        [Authorize]
        public async Task<ActionResult> DownloadAsync(long id, [FromServices] ICommandBus commandBus,
            [FromServices] IBlobProvider blobProvider2, CancellationToken token)
        {
            var user = _userManager.GetLongUserId(User);
            var query = new DocumentById(id, user);
            var tItem = _queryBus.QueryAsync(query, token);
            var tFiles = _blobProvider.FilesInDirectoryAsync("file-", id.ToString(), token).FirstAsync(cancellationToken: token);
            var item = await tItem;

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

            var uri = await tFiles;
            var file = uri.Segments.Last();

            Task followTask = Task.CompletedTask;
            if (item.UserId != user)
            {
                var command = new DownloadDocumentCommand(item.Id, user);
                followTask = commandBus.DispatchAsync(command, token);
            }
            var messageTask = _queueProvider.InsertMessageAsync(new UpdateDocumentNumberOfDownloads(id), token);

            await Task.WhenAll(followTask, messageTask);

            var nameToDownload = item.Title;
            var extension = Path.GetExtension(file);
            var url = await blobProvider2.GenerateDownloadLinkAsync(uri, TimeSpan.FromMinutes(30), nameToDownload + extension);
            return Redirect(url.AbsoluteUri);
        }
    }
}