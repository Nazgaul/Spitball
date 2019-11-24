﻿using System.Xml.Serialization;

namespace Cloudents.Web.Seo.Translations
{
    /// <summary>
    /// Encloses alternative links to a url for another language or locale
    /// </summary>
    public class SitemapPageTranslation
    {
        internal SitemapPageTranslation() { }

        /// <summary>
        /// Set an alternative link for a URL
        /// </summary>
        /// <param name="url">The URL to the translation page</param>
        /// <param name="language">The locale for the other resource, e.g. 'de-DE'</param>
        /// <param name="rel">Defaults to 'alternate'</param>
        public SitemapPageTranslation(string url, string language, string rel = "alternate")
        {
            Url = url;
            Language = language;
            Rel = rel;
        }


        /// <summary>
        /// The URL of the alternative language version of the URL
        /// </summary>
        [XmlAttribute("href")]
        public string Url { get; set; }


        /// <summary>
        /// Defaults to alternate
        /// </summary>
        [XmlAttribute("rel")]
        public string Rel { get; set; }


        /// <summary>
        /// The locale of the alternative version, e.g. de-DE
        /// </summary>
        [XmlAttribute("hreflang")]
        public string Language { get; set; }

    }
}
