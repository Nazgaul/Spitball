using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Controllers
{
    [Route("[controller]"), ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class ChatController : Controller
    {
        // GET

        [Route("download/{chatRoomId:guid}/{chatId:guid}", Name = "ChatDownload")]
        public async Task<IActionResult> Download(Guid chatRoomId, Guid chatId, [FromServices] IChatDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {
            var files = await blobProvider.FilesInDirectoryAsync("file-", $"{chatRoomId}/{chatId}", token);
            var uri = files.First();

            var url = blobProvider.GenerateDownloadLink(uri, TimeSpan.FromMinutes(30));
            return Redirect(url.AbsoluteUri);
        }
    }
}