using Cloudents.Core.Storage;
using Cloudents.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Controllers
{
    public class HomeController : Controller
    {

        //[Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }


        [Route("document/download/{id:long}", Name = "DocumentDownload"), ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DownloadDocument(long id, [FromServices] IQueryBus queryBus,
            [FromServices] IDocumentDirectoryBlobProvider blobProvider,
            [FromServices] IBlobProvider blobProvider2,
            CancellationToken token)
        {
            var files = await blobProvider.FilesInDirectoryAsync("file-", id.ToString(), token);
            var file = files.First();

            //blob.core.windows.net/spitball-files/files/6160/file-82925b5c-e3ba-4f88-962c-db3244eaf2b2-advanced-linux-programming.pdf
            var extension = Path.GetExtension(file.Segments.Last());
            var url = blobProvider2.GenerateDownloadLink(file, TimeSpan.FromMinutes(30), "Temp" + extension);
            return Redirect(url.AbsoluteUri);
        }
    }
}
