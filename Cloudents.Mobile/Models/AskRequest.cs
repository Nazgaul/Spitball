﻿using System.ComponentModel;
using Cloudents.Core.Enum;

namespace Cloudents.Mobile.Models
{
    /// <summary>
    /// Ask Question search object
    /// </summary>
    public class AskRequest : IPaging
    {
      
        /// <summary>
        /// Page for paging
        /// </summary>
        [DefaultValue(0)]
        public int? Page { get; set; }
        /// <summary>
        /// The term array of Ai parse
        /// </summary>
        public string[] Term { get; set; }
        /// <summary>
        /// Array of sites to search for
        /// </summary>
        public string[] Source { get; set; }

        /// <summary>
        /// Format of result
        /// </summary>
        [DefaultValue(0)]
        public BingTextFormat Format { get; set; }
    }
}