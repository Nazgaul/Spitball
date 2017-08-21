using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using Abot.Core;
using Abot.Poco;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class CrawlSiteMapFinder : IHyperLinkParser
    {
        private readonly HyperLinkParser m_LinkParser;
        public CrawlSiteMapFinder()
        {
            m_LinkParser = new AngleSharpHyperlinkParser();
        }

        public IEnumerable<Uri> GetLinks(CrawledPage crawledPage)
        {
            //var xmlContentTypes = new[] {
            //    "text/xml","application/xml", "application/xml; charset=utf-8"
            //};
            if (crawledPage.Uri.AbsoluteUri.ToLowerInvariant().Contains("xml"))
            //if (xmlContentTypes.Contains(crawledPage.HttpWebResponse.ContentType))
            {
                var text = crawledPage.Content.Text;
                if (crawledPage.Uri.AbsoluteUri.ToLowerInvariant().Contains("gz"))
                {
                    var t = Compress.DecompressFromGzip(crawledPage.Content.Bytes);
                    text = Encoding.UTF8.GetString(t);
                }

                //text = text.Replace("<html>", string.Empty)
                //    .Replace("<body>", string.Empty)
                //    .Replace("<head>", string.Empty)
                //    .Replace("</head>", string.Empty)
                //    .Replace("</html>", string.Empty)
                //    .Replace("</body>", string.Empty);
                var xml = new XmlDocument();
                xml.LoadXml(text);

                var siteMapElements = xml.GetElementsByTagName("sitemap");

                if (siteMapElements.Count == 0)
                {
                    siteMapElements = xml.GetElementsByTagName("url");
                }

                foreach (XmlNode siteMap in siteMapElements)
                {
                    var xmlElement = siteMap["loc"];
                    var element = siteMap["lastmod"];
                    if (element != null)
                    {
                        //var dateTime = XmlConvert.ToDateTime(element.InnerText,
                        //    XmlDateTimeSerializationMode.RoundtripKind);
                    }
                    if (xmlElement != null) yield return new Uri(xmlElement.InnerText);
                }
            }
            var htmlLinks = m_LinkParser.GetLinks(crawledPage);

            foreach (var htmlLink in htmlLinks)
            {
                yield return htmlLink;
            }
        }
    }
}