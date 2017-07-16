using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using Abot.Core;
using Abot.Poco;

namespace ConsoleAppCrawler
{
    public class SiteMapFinder : IHyperLinkParser
    {
        private readonly HyperLinkParser m_LinkParser;
        public SiteMapFinder()
        {
            m_LinkParser = new AngleSharpHyperlinkParser();
        }


        private byte[] DecompressFromGzip(byte[] byteArray)
        {
            using (var input = new MemoryStream(byteArray))
            {
                using (var output = new MemoryStream())
                {
                    using (Stream cs = new GZipStream(input, CompressionMode.Decompress))
                    {
                        cs.CopyTo(output);
                    }

                    var result = output.ToArray();
                    return result;
                }
            }
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
                    var t = DecompressFromGzip(crawledPage.Content.Bytes);
                    text = Encoding.UTF8.GetString(t);
                }

                text = text.Replace("<html>", string.Empty)
                    .Replace("<body>", string.Empty)
                    .Replace("<head>", string.Empty)
                    .Replace("</head>", string.Empty)
                    .Replace("</html>", string.Empty)
                    .Replace("</body>", string.Empty);
                var xml = new XmlDocument();
                xml.LoadXml(text);

                var siteMapElements = xml.GetElementsByTagName("sitemap");


                //if (xml.DocumentElement == null) return new Uri[] { };
                //var manager = new XmlNamespaceManager(xml.NameTable);
                //manager.AddNamespace("s", xml.DocumentElement.NamespaceURI);
                //manager.AddNamespace("v", xml.DocumentElement.NamespaceURI);

                //var links = xml.SelectNodes("/s:sitemapindex/s:sitemap", manager);

                //if (links == null) return new Uri[] { };

                if (siteMapElements.Count == 0)
                {
                    siteMapElements = xml.GetElementsByTagName("url");
                    //foreach (XmlNode url in xml.GetElementsByTagName("url"))
                    //{
                    //    var xmlElement = url["loc"];
                    //    if (xmlElement != null) yield return new Uri(xmlElement.InnerText);
                    //}
                    //    var links2 = xml.SelectNodes("/v:urlset/v:url/v:loc", manager);
                    //    if (links2 == null) return new Uri[] { };
                    //    var result2 = links2
                    //        .Cast<XmlNode>()
                    //        .Select(x => new Uri(x.InnerText));
                    //    Console.WriteLine("Going to process " + links2.Count);
                    //    return result2;
                }

                foreach (XmlNode siteMap in siteMapElements)
                {
                    var xmlElement = siteMap["loc"];
                    var element = siteMap["lastmod"];
                    if (element != null)
                    {
                        var dateTime = XmlConvert.ToDateTime(element.InnerText,
                            XmlDateTimeSerializationMode.RoundtripKind);
                    }
                    if (xmlElement != null) yield return new Uri(xmlElement.InnerText);
                }
                //var result = links
                //    .Cast<XmlNode>()
                //    .Select(x => new Uri(x.InnerText));
                //return result;




            }
            //return new Uri[] { };
            var htmlLinks = m_LinkParser.GetLinks(crawledPage);

            foreach (var htmlLink in htmlLinks)
            {
                yield return htmlLink;
            }

        }
    }
}