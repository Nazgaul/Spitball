using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Help")]
    public class HelpController : Controller
    {
        private readonly IRestClient _restClient;

        public HelpController(IRestClient blobProvider)
        {
            _restClient = blobProvider;
        }

        [HttpGet]
        [ResponseCache(Duration = TimeConst.Hour)]
        public async Task<IActionResult> GetAsync(CancellationToken token)
        {
            var uri = new Uri("https://zboxstorage.blob.core.windows.net/zboxhelp/new/help.xml");
            var t = await _restClient.DownloadStreamAsync(uri, token).ConfigureAwait(false);

            using(var stream = t.stream)
            {
                var model = PassXmlDoc(stream);
                return Json(model);
            }
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