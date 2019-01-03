using Cloudents.Web.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using SimpleMvcSitemap;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Query;
using Cloudents.Query.Query;

namespace Cloudents.Web.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    public class SiteMapController : Controller
    {
        private readonly IQueryBus _queryBus;

        public SiteMapController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [Route("sitemap.xml")]
        [ResponseCache(Duration = 2 * TimeConst.Day)]
        public async Task<IActionResult> IndexAsync(CancellationToken token)
        {
            //List<SitemapNode> nodes = new List<SitemapNode>
            //{
            //    new SitemapNode(Url.Action("Index","Home")),
            //   // new SitemapNode(Url.Action("About","Home")),
            //    //other nodes
            //};

            var sitemapIndexNodes = new List<SitemapIndexNode>();
            var query = new EmptyQuery();
            var result = await _queryBus.QueryAsync(query, token);
            foreach (var mapCountDto in result)
            {
                for (var i = 0; i <= mapCountDto.Count / 50000; i++)
                {
                    sitemapIndexNodes.Add(new SitemapIndexNode(Url.RouteUrl("siteMapDescription", new
                    {
                        type = mapCountDto.Type,
                        index = i
                    })));
                }
            }
            return new SitemapProvider().CreateSitemapIndex(new SitemapIndexModel(sitemapIndexNodes));
        }

        [Route("sitemap-flashcard-{index:int}.xml",Order = 1)]
        public async Task FlashcardSeoAsync(int index,
            [FromServices] IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery> query2,
            CancellationToken token)
        {
            // var entities = _seoRepositories[type].Get(query);
            //var routeName = type.GetDescription();
            var response = HttpContext.Response;
            response.StatusCode = 200;
            response.ContentType = "application/xml";

            var writer = XmlWriter.Create(Response.Body, new XmlWriterSettings
            {
                Async = true
            });
            await writer.WriteStartDocumentAsync();
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            var iterator = 0;
            var query = new SeoQuery(index);
            foreach (var entity in query2.Get(query))
            {
                //iterator++;
                var url = Url.RouteUrl(SeoTypeString.Flashcard, new
                {
                    universityName = UrlConst.NameToQueryString(entity.UniversityName ?? "my"),
                    boxId = entity.BoxId,
                    boxName = UrlConst.NameToQueryString(entity.BoxName),
                    id = entity.Id,
                    name = UrlConst.NameToQueryString(entity.Name)
                }, Request.GetUri().Scheme);

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
            await response.Body.FlushAsync(token);
        }

        [Route("sitemap-{type}-{index:int}.xml", Name = "siteMapDescription", Order = 2)]
        //[ResponseCache(Duration = 2 * TimeConst.Day,Va)]
        public IActionResult DetailIndexAsync(SeoType type, int index,
            [FromServices] IStatelessSession session,
            CancellationToken token)
        {
            if (type == SeoType.Flashcard)
            {
                return Ok();
            }
            var siteMap = new DocumentSiteMapIndexConfiguration(index, session, Url);
            return new DynamicSitemapIndexProvider().CreateSitemapIndex(new SitemapProvider(), siteMap);


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