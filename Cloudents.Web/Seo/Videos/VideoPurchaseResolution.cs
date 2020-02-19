﻿using System.Xml.Serialization;

namespace Cloudents.Web.Seo.Videos
{
    /// <summary>
    /// Resolution for the specified video
    /// </summary>
    public enum VideoPurchaseResolution
    {
        /// <summary>
        /// Default value, won't be serialized.
        /// </summary>
        None,

        /// <summary>
        /// HD resolution.
        /// </summary>
        [XmlEnum("hd")]
        Hd,

        /// <summary>
        /// SD resolution.
        /// </summary>
        [XmlEnum("sd")]
        Sd
    }
}