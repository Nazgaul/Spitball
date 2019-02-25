using System;
using System.Collections.Generic;
using System.Globalization;
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
    [Route("api/Help"), ApiController]
    public class HelpController : ControllerBase
    {
        private readonly IRestClient _restClient;

        public HelpController(IRestClient blobProvider)
        {
            _restClient = blobProvider;
        }

        [HttpGet]
        [ResponseCache(Duration = TimeConst.Hour, VaryByQueryKeys = new[] { "*" })]
        public async Task<IEnumerable<QnA>> GetAsync(CancellationToken token)
        {
            var uri = new Uri("https://zboxstorage.blob.core.windows.net/zboxhelp/new/help2.xml");
            var t = await _restClient.DownloadStreamAsync(uri, token);

            using (var stream = t.stream)
            {
                var model = PassXmlDoc(stream);
                return model;
            }
        }

        [NonAction]
        public static IEnumerable<QnA> PassXmlDoc(Stream stream)
        {
            var data = XDocument.Load(stream);
            return from content in data.Descendants("content")
                where string.Equals((content.Attribute("lang")?.Value ?? "en"),
                    CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase)
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