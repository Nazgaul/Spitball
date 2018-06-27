using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Help")]
    public class HelpController : Controller
    {
        private readonly IRestClient _blobProvider;

        public HelpController(IRestClient blobProvider)
        {
            _blobProvider = blobProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken token)
        {
            var uri = new Uri("https://zboxstorage.blob.core.windows.net/zboxhelp/new/help.xml");
            var t = await _blobProvider.DownloadStreamAsync(uri, token);

            using(var stream = t.stream)
            {
                var model = PassXmlDoc(stream);
                return Json(model);
            }
            //using (var stream = await _blobProvider.DownloadFileAsync(uri, token).ConfigureAwait(false))
            //{
            //    var model = PassXmlDoc(stream);
            //    return Json(model);
            //}
        }
        [NonAction]
        public static IEnumerable<QnA> PassXmlDoc(Stream stream)
        {
            var data = XDocument.Load(stream);
            return from content in data.Descendants("content")
                orderby int.Parse(content.Attribute("order")?.Value ?? "0")
                select new QnA
                {
                    Question = content.Element("question")?.Value.Trim(),
                    Answer = content.Element("answer")?.Value.Trim()

                };
        }

        public class QnA
        {
            public string Question { get; set; }
            public string Answer { get; set; }
        }
    }
}