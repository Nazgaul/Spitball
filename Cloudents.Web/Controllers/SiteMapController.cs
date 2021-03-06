﻿using Autofac.Features.Indexed;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Query;
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
using Cloudents.Web.Services;
using Cloudents.Query.General;

namespace Cloudents.Web.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    public class SiteMapController : Controller
    {
        internal const int PageSize = 20000;
        private readonly IQueryBus _queryBus;
        private readonly ConfigurationService _configurationService;

        private readonly XmlWriterSettings _xmlWriterSettings = new XmlWriterSettings
        {
            Async = true,
            Encoding = Encoding.UTF8,
            Indent = true,
            OmitXmlDeclaration = true,
            IndentChars = "  ",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };

        public SiteMapController(IQueryBus queryBus, ConfigurationService configurationService)
        {
            _queryBus = queryBus;
            _configurationService = configurationService;
        }


        [Route("sitemap.xml")]
        [ResponseCache(Duration = 1 * TimeConst.Day)]
        public async Task<IActionResult> IndexAsync(CancellationToken token)
        {
            var query = new SiteMapQuery(_configurationService.GetSiteName() == ConfigurationService.Site.Frymo);
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
            return new FileCallbackResult("application/xml", (stream, context) => document.SaveAsync(stream, SaveOptions.OmitDuplicateNamespaces, token));
        }



        [Route("sitemap-{type}-{index:int}.xml", Name = "siteMapDescription")]
        public IActionResult DetailIndex(SeoType type, int index,
            [FromServices] IIndex<SeoType, IBuildSeo> seoBuilder)
        {
            var provider = seoBuilder[type];
            var urls = provider.GetUrls(_configurationService.GetSiteName() == ConfigurationService.Site.Frymo, index);

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            return new FileCallbackResult("application/xml", async (stream, context) =>
            {
                using var writer = XmlWriter.Create(stream, _xmlWriterSettings);
                var serializer = new XmlSerializer(typeof(SitemapNode));
                var i = 0;
                await writer.WriteStartDocumentAsync();

                await writer.WriteStartElementAsync(null, "urlset", null);
                await writer.WriteAttributeStringAsync("xmlns", "image", null, "http://www.google.com/schemas/sitemap-image/1.1");
                await writer.WriteAttributeStringAsync("v", "xmlns", null, "http://www.sitemaps.org/schemas/sitemap/0.9");
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
            });
        }

        
    }
}