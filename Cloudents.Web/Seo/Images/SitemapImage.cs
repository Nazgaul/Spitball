﻿using System.Xml.Serialization;

namespace Cloudents.Web.Seo.Images
{
    /// <summary>
    /// Encloses all information about a single image
    /// </summary>
    [XmlType(Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public class SitemapImage
    {
        internal SitemapImage() { }

        /// <summary>
        /// Creates an instance of SitemapImage
        /// </summary>
        /// <param name="url">The URL of the image.</param>
        public SitemapImage(string url)
        {
            Url = url;
        }


        /// <summary>
        /// The URL of the image.
        /// </summary>
        [XmlElement("loc", Order = 1)]
        public string Url { get; set; }


        /// <summary>
        /// The caption of the image.
        /// </summary>
        [XmlElement("caption", Order = 2)]
        public string Caption { get; set; }


        /// <summary>
        /// The geographic location of the image.
        /// </summary>
        [XmlElement("geo_location", Order = 3)]
        public string Location { get; set; }


        /// <summary>
        /// The title of the image.
        /// </summary>
        [XmlElement("title", Order = 4)]
        public string Title { get; set; }


        /// <summary>
        /// A URL to the license of the image.
        /// </summary>
        [XmlElement("license", Order = 5)]
        public string License { get; set; }

    }
}
