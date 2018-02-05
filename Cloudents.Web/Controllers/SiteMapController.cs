using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Autofac.Features.Indexed;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class SiteMapController : Controller
    {
        private readonly IReadRepositoryAsync<IEnumerable<SiteMapCountDto>> _readRepository;
        private readonly IIndex<SeoType, IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>> _seoRepositories;

        public SiteMapController(IReadRepositoryAsync<IEnumerable<SiteMapCountDto>> readRepository, IIndex<SeoType, IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>> seoRepositories)
        {
            _readRepository = readRepository;
            _seoRepositories = seoRepositories;
        }

        [Route("sitemap.xml")]
        [ResponseCache(Duration = 2 * TimeConst.Day)]
        public async Task<IActionResult> IndexAsync(CancellationToken token)
        {
            XNamespace nameSpace = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var model = await _readRepository.GetAsync(token).ConfigureAwait(false);

            const int pageSize = 49950;
            // ReSharper disable once StringLiteralTypo
            var root = new XElement(nameSpace + "sitemapindex");
            foreach (var elem in model)
            {
                for (var i = 0; i <= elem.Count / pageSize; i++)
                {
                    var url = Url.RouteUrl("siteMapDescription", new { type = elem.Type, index = i }, Request.GetUri().Scheme);
                    root.Add(
                        new XElement(nameSpace + "sitemap",
                            new XElement(nameSpace + "loc",
                                url)
                        )
                    );
                }
            }
            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            return Content(document.ToString(), "application/xml");
        }

        [Route("sitemap-{type}-{index:int}.xml", Name = "siteMapDescription")]
        //[ResponseCache(Duration = 2 * TimeConst.Day,Va)]
        public async Task DetailIndexAsync(SeoType type, int index, CancellationToken token)
        {
            var query = new SeoQuery(index);
            var entities = _seoRepositories[type].Get(query);
            var routeName = type.GetDescription();
            var response = HttpContext.Response;
            response.StatusCode = 200;
            response.ContentType = "application/xml";

            var writer = XmlWriter.Create(Response.Body, new XmlWriterSettings
            {
                Async = true
            });
            await writer.WriteStartDocumentAsync().ConfigureAwait(false);
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            var iterator = 0;
            foreach (var entity in entities)
            {
                iterator++;
                var url = Url.RouteUrl(routeName, new
                {
                    universityName = UrlConst.NameToQueryString(entity.UniversityName ?? "my"),
                    boxId = entity.BoxId,
                    boxName = UrlConst.NameToQueryString(entity.BoxName),
                    id = entity.Id,
                    name = UrlConst.NameToQueryString(entity.Name)
                }, Request.GetUri().Scheme);

                await WriteTagAsync("1", "Daily", url, writer).ConfigureAwait(false);
                if (iterator == 100)
                {
                    await writer.FlushAsync().ConfigureAwait(false);
                    iterator = 0;
                }
            }
            await writer.WriteEndElementAsync().ConfigureAwait(false);
            await writer.WriteEndDocumentAsync().ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);
            await response.Body.FlushAsync(token).ConfigureAwait(false);
        }

        private static async Task WriteTagAsync(string priority, string freq,
            string navigation, XmlWriter myWriter)
        {
            myWriter.WriteStartElement("url");

            myWriter.WriteStartElement("loc");
            myWriter.WriteValue(navigation);
            await myWriter.WriteEndElementAsync().ConfigureAwait(false);

            myWriter.WriteStartElement("lastmod");
            myWriter.WriteValue(DateTime.Now.ToString("yyyy-MM-dd"));
            await myWriter.WriteEndElementAsync().ConfigureAwait(false);

            myWriter.WriteStartElement("changefreq");
            myWriter.WriteValue(freq);
            await myWriter.WriteEndElementAsync().ConfigureAwait(false);

            myWriter.WriteStartElement("priority");
            myWriter.WriteValue(priority);
            await myWriter.WriteEndElementAsync().ConfigureAwait(false);

            await myWriter.WriteEndElementAsync().ConfigureAwait(false);
        }
    }
}