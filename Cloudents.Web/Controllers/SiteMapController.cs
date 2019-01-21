using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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
        [ResponseCache(Duration = 1 * TimeConst.Day)]
        public async Task<IActionResult> IndexAsync(CancellationToken token)
        {
            //List<SitemapNode> nodes = new List<SitemapNode>
            //{
            //    new SitemapNode(Url.Action("Index","Home")),
            //   // new SitemapNode(Url.Action("About","Home")),
            //    //other nodes
            //};
           
            

            //var sitemapIndexNodes = new List<SitemapIndexNode>();
            var query = new EmptyQuery();
            var result = await _queryBus.QueryAsync(query, token);

            return new FileCallbackResult("application/xml", async (stream, context) =>
            {
                var writer = XmlWriter.Create(stream, new XmlWriterSettings
                {
                    Async = true
                });
                await writer.WriteStartDocumentAsync();
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                const int pageSize = 50000;
                foreach (var elem in result)
                {
                    for (var i = 0; i <= elem.Count / pageSize; i++)
                    {

                        writer.WriteStartElement("sitemap");
                        writer.WriteStartElement("loc");



                        var url = Url.RouteUrl("siteMapDescription", new { type = elem.Type, index = i },
                            Request.GetUri().Scheme);
                        writer.WriteValue(url);
                        await writer.WriteEndElementAsync();
                        await writer.WriteEndElementAsync();
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                await writer.FlushAsync();






            });

            //    foreach (var mapCountDto in result)
            //{
            //    for (var i = 0; i <= mapCdffrdwountDto.Count / 50000; i++)
            //    {das
            //        sitemapIndexNodes.Add(new SitemapIndexNode(Url.RouteUrl("siteMapDescription", new
            //        {as
            //            type = mapCountDto.Type,
            //            index = i
            //        })));
            //    }
            //}
            //return new SitemapProvider().CreateSitemapIndex(new SitemapIndexModel(sitemapIndexNodes));
        }

        [Route("sitemap-flashcard-{index:int}.xml",Order = 1)]
        public async Task<IActionResult> FlashcardSeoAsync(int index,
            [FromServices] IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery> query2,
            CancellationToken token)
        {
           
            // var entities = _seoRepositories[type].Get(query);
            //var routeName = type.GetDescription();

            return new FileCallbackResult("application/xml", async (stream, context) =>
            {
                var writer = XmlWriter.Create(stream, new XmlWriterSettings
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
            });
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


          var t =   session.Query<Document>()
                .Where(w => w.Status.State == ItemState.Ok)
                .Fetch(f => f.University)
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
              var writer = XmlWriter.Create(stream, new XmlWriterSettings
              {
                  Async = true
              });
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
                  },Request.GetUri().Scheme);
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
          });
              
              //return new SitemapNode(_urlHelper.RouteUrl(SeoTypeString.Document, new
              //{
              //    universityName = FriendlyUrlHelper.GetFriendlyTitle(source.UniversityName),
              //    courseName = FriendlyUrlHelper.GetFriendlyTitle(source.CourseName),
              //    source.Id,
              //    name = FriendlyUrlHelper.GetFriendlyTitle(source.Name)
              //}));
          }

            //var siteMap = new DocumentSiteMapIndexConfiguration(index, session, Url);
            //return new DynamicSitemapIndexProvider().CreateSitemapIndex(new SitemapProvider(), siteMap);


        

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