using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Cloudents.Web.Controllers
{
    public class SiteMapController : Controller
    {
        private readonly IReadRepositoryAsync<IEnumerable<SiteMapCountDto>> _readRepository;

        public SiteMapController(IReadRepositoryAsync<IEnumerable<SiteMapCountDto>> readRepository)
        {
            _readRepository = readRepository;
        }

        [Route("sitemap.xml")]
        [ResponseCache(Duration = 2 * TimeConst.Day)]
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var content = await GetSiteMapIndexAsync(token).ConfigureAwait(false);
            return Content(content, "application/xml");
        }

        [Route("sitemap-{type}-{index:int}.xml")]
        //[ResponseCache(Duration = 2 * TimeConst.Day,Va)]
        public async Task DetailIndexAsync(SeoType type, int index, CancellationToken token)
        {
            //var content = await GetSiteMapXmlAsync(type, index,token).ConfigureAwait(false);

            var response = HttpContext.Response;

            response.StatusCode = 200;
            response.ContentType = "application/xml";

            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace xhtml = "http://www.w3.org/1999/xhtml";
            IEnumerable<SiteMapNode> nodes;


            //    if (type == SeoType.Static)
            //    {
            //        //nodes = GetSitemapStaticLinks();
            //    }
            //    else
            //    {
            //        nodes = await GetSitemapNodesAsync(type, index, cancellationToken).ConfigureAwait(false);
            //    }
            var writer = XmlWriter.Create(Response.Body,new XmlWriterSettings
            {
                Async = true
            });
            await writer.WriteStartDocumentAsync().ConfigureAwait(false);
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            WriteTag("1", "Daily", "http://www.delshad.ir/default.aspx", writer);
            WriteTag("0.6", "Yearly", "http://www.delshad.ir/Contact.aspx", writer);
            WriteTag("0.8", "Monthly", "http://www.delshad.ir/About.aspx", writer);

            await writer.WriteEndElementAsync().ConfigureAwait(false);
            await writer.WriteEndDocumentAsync().ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);
            //var p = new XmlTextWriter(response.Body,Encoding.UTF8);
            

            //var root = new XElement(xmlns + "urlset",
            //    new XAttribute(XNamespace.Xmlns + "xhtml", xhtml));

            //    foreach (var node in nodes)
            //    {
            //        cancellationToken.ThrowIfCancellationRequested();
            //        var locContent = new XElement(xmlns + "loc", node.Url);
            //        var priorityContent = node.Priority == null
            //                ? null
            //                : new XElement(xmlns + "priority",
            //                    node.Priority.Value.ToString("F1", CultureInfo.InvariantCulture));
            //        var lastmodContent = node.LastModified == null
            //            ? null
            //            : new XElement(xmlns + "lastmod",
            //                // ReSharper disable once StringLiteralTypo
            //                node.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"));
            //        var frequencyContent = node.Frequency == null
            //            ? null
            //            : new XElement(xmlns + "changefreq", node.Frequency.Value.ToString().ToLowerInvariant());

            //        var url = new XElement(xmlns + "url", locContent, priorityContent, lastmodContent, frequencyContent);
            //        if (node.SitemapLangNodes != null)
            //        {
            //            foreach (var lang in node.SitemapLangNodes)
            //            {
            //                var langNode = new XElement(xhtml + "link",
            //                new XAttribute("rel", "alternate"),
            //                new XAttribute("hreflang", lang.Language),
            //                new XAttribute("href", lang.Url));

            //                url.Add(langNode);
            //            }
            //        }

            //        root.Add(url);
            //    }
            //    var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            //    return document.ToString();

            //for (var i = 0; i < 10; ++i)
            //{
            //    //the tags are either 'events:' or 'data:' and two \n indicates ends of the msg
            //    //event: xyz \n\n
            //    //data: xyz \n\n

            //    await response.WriteAsync($"data: test {i}\n\n", cancellationToken: token).ConfigureAwait(false);

            //    await response.Body.FlushAsync(token).ConfigureAwait(false);
            //    await Task.Delay(5 * 1000, token).ConfigureAwait(false);
            //}
            //await response.WriteAsync("data:\n\n", cancellationToken: token).ConfigureAwait(false);
            await response.Body.FlushAsync(token).ConfigureAwait(false);
            //return Content(content, "application/xml");
        }


        private void WriteTag(string Priority, string freq,
            string Navigation, XmlWriter MyWriter)
        {
            MyWriter.WriteStartElement("url");

            MyWriter.WriteStartElement("loc");
            MyWriter.WriteValue(Navigation);
            MyWriter.WriteEndElement();

            MyWriter.WriteStartElement("lastmod");
            MyWriter.WriteValue(DateTime.Now.ToShortDateString());
            MyWriter.WriteEndElement();

            MyWriter.WriteStartElement("changefreq");
            MyWriter.WriteValue(freq);
            MyWriter.WriteEndElement();

            MyWriter.WriteStartElement("priority");
            MyWriter.WriteValue(Priority);
            MyWriter.WriteEndElement();

            MyWriter.WriteEndElement();
        }


        //private async Task<string> GetSiteMapXmlAsync(SeoType type, int index, CancellationToken cancellationToken)
        //{
        //    XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        //    XNamespace xhtml = "http://www.w3.org/1999/xhtml";
        //    IEnumerable<SitemapNode> nodes;
        //    if (type == SeoType.Static)
        //    {
        //        //nodes = GetSitemapStaticLinks();
        //    }
        //    else
        //    {
        //        nodes = await GetSitemapNodesAsync(type, index, cancellationToken).ConfigureAwait(false);
        //    }

        //    var root = new XElement(xmlns + "urlset",
        //        new XAttribute(XNamespace.Xmlns + "xhtml", xhtml));

        //    foreach (var node in nodes)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();
        //        var locContent = new XElement(xmlns + "loc", node.Url);
        //        var priorityContent = node.Priority == null
        //                ? null
        //                : new XElement(xmlns + "priority",
        //                    node.Priority.Value.ToString("F1", CultureInfo.InvariantCulture));
        //        var lastmodContent = node.LastModified == null
        //            ? null
        //            : new XElement(xmlns + "lastmod",
        //                // ReSharper disable once StringLiteralTypo
        //                node.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"));
        //        var frequencyContent = node.Frequency == null
        //            ? null
        //            : new XElement(xmlns + "changefreq", node.Frequency.Value.ToString().ToLowerInvariant());

        //        var url = new XElement(xmlns + "url", locContent, priorityContent, lastmodContent, frequencyContent);
        //        if (node.SitemapLangNodes != null)
        //        {
        //            foreach (var lang in node.SitemapLangNodes)
        //            {
        //                var langNode = new XElement(xhtml + "link",
        //                new XAttribute("rel", "alternate"),
        //                new XAttribute("hreflang", lang.Language),
        //                new XAttribute("href", lang.Url));

        //                url.Add(langNode);
        //            }
        //        }

        //        root.Add(url);
        //    }
        //    var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
        //    return document.ToString();
        //}

        private async Task<string> GetSiteMapIndexAsync(CancellationToken token)
        {
            XNamespace nameSpace = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var model = await _readRepository.GetAsync(token).ConfigureAwait(false);

            const int pageSize = 49950;
            // ReSharper disable once StringLiteralTypo
            var root = new XElement(nameSpace + "sitemapindex");
            //root.Add(
            //    new XElement(nameSpace + "sitemap",
            //        new XElement(nameSpace + "loc", $"https://www.spitball.co/sitemap-{SeoType.Static}-0.xml")
            //    )
            //);

            foreach (var elem in model)
            {
                for (var i = 0; i <= elem.Count / pageSize; i++)
                {
                    root.Add(
                        new XElement(nameSpace + "sitemap",
                            new XElement(nameSpace + "loc", $"https://www.spitball.co/sitemap-{elem.Type}-{i}.xml")
                        )
                    );
                }
            }
            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            return document.ToString();
        }
    }
}