using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Cloudents.Web.Seo.Images;


namespace Cloudents.Web.Seo
{
    /// <summary>
    /// Encloses all information about a specific URL.
    /// </summary>
    [XmlRoot("url")]
    public class SitemapNode
    {

        internal SitemapNode() { }

        /// <summary>
        /// Creates a sitemap node
        /// </summary>
        /// <param name="url">Specifies the URL. For images and video, specifies the landing page (aka play page).</param>
        public SitemapNode(string url)
        {
            Url = url;
        }

        /// <summary>
        /// URL of the page.
        /// This URL must begin with the protocol (such as http) and end with a trailing slash, if your web server requires it.
        /// This value must be less than 2,048 characters.
        /// </summary>
        [XmlElement("loc", Order = 1)]
        public string Url { get; set; }


        /// <summary>
        /// Shows the date the URL was last modified, value is optional.
        /// </summary>
        [XmlElement("lastmod", Order = 2)]
        public string LastMod
        {
            get => TimeStamp?.ToString("yyyy-MM-dd");
            set => TimeStamp = DateTime.Parse(value);
        }

        [XmlIgnore]
        public DateTime? TimeStamp { get; set; }


        /// <summary>
        /// How frequently the page is likely to change. 
        /// This value provides general information to search engines and may not correlate exactly to how often they crawl the page.
        /// </summary>
        [XmlElement("changefreq", Order = 3)]
        public ChangeFrequency? ChangeFrequency { get; set; }


        /// <summary>
        /// The priority of this URL relative to other URLs on your site. Valid values range from 0.0 to 1.0. This value does not affect how your pages are compared to pages on other sites—it only lets the search engines know which pages you deem most important for the crawlers.
        /// The default priority of a page is 0.5.
        /// Please note that the priority you assign to a page is not likely to influence the position of your URLs in a search engine's result pages.
        /// Search engines may use this information when selecting between URLs on the same site, 
        /// so you can use this tag to increase the likelihood that your most important pages are present in a search index.
        /// Also, please note that assigning a high priority to all of the URLs on your site is not likely to help you.
        /// Since the priority is relative, it is only used to select between URLs on your site.
        /// </summary>
        [XmlElement("priority", Order = 4)]
        public decimal? Priority { get; set; }


        /// <summary>
        /// Additional information about important images on the page.
        /// It can include up to 1000 images.
        /// </summary>
        [XmlElement("image", Order = 5, Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
        public List<SitemapImage> Images { get; set; }

        ///// <summary>
        ///// Additional information about news article on the page.
        ///// </summary>
        //[XmlElement("news", Order = 6, Namespace = "http://www.google.com/schemas/sitemap-news/0.9")]
        //public SitemapNews News { get; set; }


        ///// <summary>
        ///// Additional information about video content on the page.
        ///// </summary>
        //[XmlElement("video", Order = 7, Namespace = "http://www.google.com/schemas/sitemap-video/1.1")]
        //public SitemapVideo Video { get; set; }


        ///// <summary>
        ///// Specifies if the linked document is mobile friendly.
        ///// </summary>
        //[XmlElement("mobile", Order = 8, Namespace = "http://www.google.com/schemas/sitemap-mobile/1.0")]
        //[Obsolete]
        //public SitemapMobile Mobile { get; set; }


        ///// <summary>
        ///// Alternative language versions of the URL
        ///// </summary>
        //[XmlElement("link", Order = 9, Namespace = "http://www.w3.org/1999/xhtml")]
        //public List<SitemapPageTranslation> Translations { get; set; }

        /// <summary>
        /// Used for not serializing null values.
        /// </summary>
        public bool ShouldSerializeLastMod()
        {
            return TimeStamp.HasValue;
        }

        /// <summary>
        /// Used for not serializing null values.
        /// </summary>
        public bool ShouldSerializeChangeFrequency()
        {
            return ChangeFrequency.HasValue;
        }

        /// <summary>
        /// Used for not serializing null values.
        /// </summary>
        public bool ShouldSerializePriority()
        {
            return Priority.HasValue;
        }

        public XmlSerializerNamespaces GenerateSpecificNameSpace()
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "http://www.sitemaps.org/schemas/sitemap/0.9");

            if (Images?.Count > 0)
            {
                ns.Add("image", "http://www.google.com/schemas/sitemap-image/1.1");
            }

            //if (this.Video != null)
            //{
            //    ns.Add("video", "http://www.google.com/schemas/sitemap-video/1.1");
            //}

            //if (this.Translations?.Count > 0)
            //{
            //    ns.Add("xhtml", "http://www.w3.org/1999/xhtml");
            //}

            return ns;
            //ns.Add("image", "http://www.google.com/schemas/sitemap-image/1.1");
            //ns.Add("video", "http://www.google.com/schemas/sitemap-video/1.1");
            //ns.Add("xhtml", "http://www.w3.org/1999/xhtml");
        }
    }
}