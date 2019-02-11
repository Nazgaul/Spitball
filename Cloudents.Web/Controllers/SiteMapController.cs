using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Cloudents.Web.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    public class SiteMapController : Controller
    {
        private const int PageSize = 40000;
        private readonly IQueryBus _queryBus;
        private readonly XmlWriterSettings _xmlWriterSettings = new XmlWriterSettings
        {
            Async = true,
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
            var query = new EmptyQuery();
            var result = await _queryBus.QueryAsync(query, token);
            XNamespace nameSpace = "http://www.sitemaps.org/schemas/sitemap/0.9";

            // ReSharper disable once StringLiteralTypo
            var root = new XElement(nameSpace + "sitemapindex");

            foreach (var elem in result)
            {
                for (var i = 0; i <= elem.Count / PageSize; i++)
                {
                    var url = Url.RouteUrl("siteMapDescription", new { type = elem.Type, index = i },
                                        Request.GetUri().Scheme);
                    root.Add(
                         new XElement(nameSpace + "sitemap",
                             new XElement(nameSpace + "loc", url)
                         )
                     );
                }
            }
            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            return Content(document.ToString(), "application/xml");
        }

        [Route("sitemap-flashcard-{index:int}.xml", Order = 1)]
        public IActionResult FlashcardSeoAsync(int index,
            [FromServices] IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery> query2,
            CancellationToken token)
        {
            return new FileCallbackResult("application/xml", async (stream, context) =>
            {

                using (var writer = XmlWriter.Create(stream, _xmlWriterSettings))
                {
                    await writer.WriteStartDocumentAsync();
                    writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                    var iterator = 0;
                    var query = new SeoQuery(index, PageSize);
                    foreach (var entity in query2.Get(query))
                    {
                        var url = Url.RouteUrl(SeoTypeString.Flashcard, new
                        {
                            universityName = UrlConst.NameToQueryString(entity.UniversityName ?? "my"),
                            boxId = entity.BoxId,
                            boxName = UrlConst.NameToQueryString(entity.BoxName),
                            id = entity.Id,
                            name = UrlConst.NameToQueryString(entity.Name)
                        }, Request.GetUri().Scheme);
                        if (string.IsNullOrEmpty(url))
                        {
                            continue;
                        }
                        await WriteTagAsync("1", "Daily", url, writer);
                        if (iterator == 100)
                        {
                            await writer.FlushAsync();
                            iterator = 0;
                        }
                    }

                    await writer.WriteEndElementAsync();
                    await writer.WriteEndDocumentAsync();
                    await writer.FlushAsync();
                }
            });
        }

        [Route("sitemap-{type}-{index:int}.xml", Name = "siteMapDescription", Order = 2)]
        public IActionResult DetailIndexAsync(SeoType type, int index,
            [FromServices] IStatelessSession session,
            CancellationToken token)
        {
           
            var t = session.Query<Document>()
                    .Fetch(f => f.University)
                  .Where(w => w.Status.State == ItemState.Ok)
                  .Take(PageSize).Skip(PageSize * index)
                  .Select(s => new DocumentSeoDto
                  {
                      Id = s.Id,
                      Name = s.Name,
                      Country = s.University.Country,
                      CourseName = s.Course.Name,
                      UniversityName = s.University.Name
                  });
            return new FileCallbackResult("application/xml", async (stream, context) =>
            {
                using (var writer = XmlWriter.Create(stream, _xmlWriterSettings))
                {
                    var i = 0;
                    await writer.WriteStartDocumentAsync();
                    writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                    foreach (var link in t)
                    {
                        var url = Url.RouteUrl(SeoTypeString.Document, new
                        {
                            universityName = FriendlyUrlHelper.GetFriendlyTitle(link.UniversityName),
                            courseName = FriendlyUrlHelper.GetFriendlyTitle(link.CourseName),
                            link.Id,
                            name = FriendlyUrlHelper.GetFriendlyTitle(link.Name)
                        }, Request.GetUri().Scheme);
                        i++;
                        await WriteTagAsync("1", "Daily", url, writer);
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

        private static async Task WriteTagAsync(string priority, string freq,
            string navigation, XmlWriter myWriter)
        {
            myWriter.WriteStartElement("url");

            myWriter.WriteStartElement("loc");
            myWriter.WriteValue(navigation);
            await myWriter.WriteEndElementAsync();

            myWriter.WriteStartElement("lastmod");
            myWriter.WriteValue(DateTime.Now.ToString("yyyy-MM-dd"));
            await myWriter.WriteEndElementAsync();

            myWriter.WriteStartElement("changefreq");
            myWriter.WriteValue(freq);
            await myWriter.WriteEndElementAsync();

            myWriter.WriteStartElement("priority");
            myWriter.WriteValue(priority);
            await myWriter.WriteEndElementAsync();
            
            await myWriter.WriteEndElementAsync();
        }
    }
}