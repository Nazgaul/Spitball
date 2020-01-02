using Autofac.Features.Indexed;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Framework;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Cloudents.Web.Seo;


namespace Cloudents.Web.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    public class SiteMapController : Controller
    {
        internal const int PageSize = 20000;
        private readonly IQueryBus _queryBus;
        private readonly XmlWriterSettings _xmlWriterSettings = new XmlWriterSettings
        {
            Async = true,
            Encoding = Encoding.UTF8,
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };

        public SiteMapController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }


        [Route("sitemap.xml")]
        [ResponseCache(Duration = 1 * TimeConst.Day)]
        public async Task<IActionResult> IndexAsync(CancellationToken token)
        {
            var query = new SiteMapQuery();
            var result = await _queryBus.QueryAsync(query, token);
            result.Add(new SiteMapCountDto(SeoType.Static, 1));

            XNamespace nameSpace = "http://www.sitemaps.org/schemas/sitemap/0.9";

            // ReSharper disable once StringLiteralTypo
            var root = new XElement(nameSpace + "sitemapindex");

            foreach (var elem in result)
            {
                for (var i = 0; i <= elem.Count / PageSize; i++)
                {

                    var url = Url.RouteUrl("siteMapDescription",
                        new { type = elem.Type, index = i },
                                        Request.GetUri().Scheme);
                    root.Add(
                         new XElement(nameSpace + "sitemap",
                             new XElement(nameSpace + "loc", url)
                         )
                     );
                }
            }
            var document = new XDocument(
                 new XDeclaration("1.0", "utf-8", ""), root);
            return new FileCallbackResult("application/xml",
                async (stream, context) =>
                {
                    await document.SaveAsync(stream, SaveOptions.OmitDuplicateNamespaces, token);
                });
        }



        [Route("sitemap-{type}-{index:int}.xml", Name = "siteMapDescription")]
        public IActionResult DetailIndexAsync(SeoType type, int index,
            [FromServices] IIndex<SeoType, IBuildSeo> seoBuilder)
        {
            var provider = seoBuilder[type];
            var urls = provider.GetUrls(index);

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            return new FileCallbackResult("application/xml", async (stream, context) =>
            {
                using (var writer = XmlWriter.Create(stream, _xmlWriterSettings))
                {
                    var serializer = new XmlSerializer(typeof(SitemapNode), "");
                    var i = 0;
                    await writer.WriteStartDocumentAsync();

                    //writer.WriteStartElement();
                    writer.WriteStartElement("urlset",
                        "http://www.sitemaps.org/schemas/sitemap/0.9");
                    writer.WriteAttributeString("xmlns", "image", null, "http://www.google.com/schemas/sitemap-image/1.1");
                    //writer.WriteAttributeString("xmlns", "video", null, "http://www.google.com/schemas/sitemap-video/1.1");
                    //writer.WriteAttributeString("xmlns", "xhtml", null, "http://www.w3.org/1999/xhtml");
                    //writer.WriteAttributeString("xmlns:xhtml", "http://www.w3.org/1999/xhtml"); // for lang support
                    foreach (var url in urls)
                    {
                        i++;
                        serializer.Serialize(writer, url, ns);
                        if (i % 100 == 0)
                        {
                            await writer.FlushAsync();
                        }
                    }

                    await writer.WriteEndElementAsync();
                    await writer.WriteEndDocumentAsync();
                    await writer.FlushAsync();
                }
            });
        }

        //private static async Task WriteTagAsync(string priority, string freq,
        //    string navigation, XmlWriter myWriter)
        //{
        //    myWriter.WriteStartElement("url");

        //    myWriter.WriteStartElement("loc");
        //    myWriter.WriteValue(navigation);
        //    await myWriter.WriteEndElementAsync();

        //    myWriter.WriteStartElement("lastmod");
        //    myWriter.WriteValue(DateTime.Now.ToString("yyyy-MM-dd"));
        //    await myWriter.WriteEndElementAsync();

        //    myWriter.WriteStartElement("changefreq");
        //    myWriter.WriteValue(freq);
        //    await myWriter.WriteEndElementAsync();

        //    myWriter.WriteStartElement("priority");
        //    myWriter.WriteValue(priority);
        //    await myWriter.WriteEndElementAsync();

        //    await myWriter.WriteEndElementAsync();
        //}
    }
}