using Autofac.Features.Indexed;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Cloudents.Web.Framework;

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
            var query = new SiteMapQuery();
            var result = await _queryBus.QueryAsync(query, token);

            XNamespace nameSpace = "http://www.sitemaps.org/schemas/sitemap/0.9";

            result.Add(new SiteMapCountDto(SeoType.Static, 1));
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

       

        [Route("sitemap-{type}-{index:int}.xml", Name = "siteMapDescription", Order = 2)]
        public IActionResult DetailIndexAsync(SeoType type, int index,
            [FromServices] IIndex<SeoType, IBuildSeo> seoBuilder,
        CancellationToken token)
        {
            var provider = seoBuilder[type];
            var x = provider.GetUrls(index);

            return new FileCallbackResult("application/xml", async (stream, context) =>
            {
                using (var writer = XmlWriter.Create(stream, _xmlWriterSettings))
                {
                    var i = 0;
                    await writer.WriteStartDocumentAsync();
                    writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                    foreach (var url in x)
                    {
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

    public interface IBuildSeo
    {
        IEnumerable<string> GetUrls(int index);


    }

    public class StaticSeoBuilder : IBuildSeo
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StaticSeoBuilder(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<string> GetUrls(int index)
        {
            yield return _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext, "Index", "Home");
            yield return _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, "Static", new
            {
                page = "faq"
            });
        }
    }

    public class DocumentSeoBuilder : IBuildSeo
    {
        private readonly IStatelessSession _session;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DocumentSeoBuilder(IStatelessSession session, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _session = session;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<string> GetUrls(int index)
        {
            var t = _session.Query<Document>()
                 .Fetch(f => f.University)
                 .Where(w => w.Status.State == ItemState.Ok)
                 .Take(4000).Skip(4000 * index)
                 .Select(s => new DocumentSeoDto
                 {
                     Id = s.Id,
                     Name = s.Name,
                     Country = s.University.Country,
                     CourseName = s.Course.Id,
                     UniversityName = s.University.Name
                 });

            foreach (var item in t)
            {
                yield return _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Document, new
                {
                    universityName = FriendlyUrlHelper.GetFriendlyTitle(item.UniversityName),
                    courseName = FriendlyUrlHelper.GetFriendlyTitle(item.CourseName),
                    item.Id,
                    name = FriendlyUrlHelper.GetFriendlyTitle(item.Name)
                });

            }
        }
    }


    public class QuestionSeoBuilder : IBuildSeo
    {
        private readonly IStatelessSession _session;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionSeoBuilder(IStatelessSession session, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _session = session;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<string> GetUrls(int index)
        {
            var t = _session.Query<Question>()
                .Where(w => w.Status.State == ItemState.Ok)
                .Take(4000).Skip(4000 * index)
                .Select(s => s.Id);

            foreach (var item in t)
            {
                yield return _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Question, new
                {
                    Id = item,
                });

            }
        }
    }
}