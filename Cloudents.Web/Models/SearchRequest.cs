﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Document and flashcard search object
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Api model")]
    public class SearchRequest : IPaging
    {
        /// <summary>
        /// User courses id
        /// </summary>
        public long[] Course { get; set; }

        /// <summary>
        /// Format of result
        /// </summary>
        [DefaultValue(0)]
        public HighlightTextFormat Format { get; set; }

        /// <summary>
        /// Page for paging
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// The term array of Ai parse
        /// </summary>
       // [DisplayFormat(HtmlEncode = true)]
        public string Query { get; set; }

        /// <summary>
        /// User sort option
        /// </summary>
        [DefaultValue(0)]
        public SearchRequestSort? Sort { get; set; }

        /// <summary>
        /// Array of sites to search for
        /// </summary>

        [DisplayFormat(HtmlEncode = true)]
        public string[] Source { get; set; }

        /// <summary>
        /// User university id
        /// </summary>
        [Range(1, long.MaxValue)]
        public long? University { get; set; }

        //[BindNever]
        public GeographicCoordinate GeoPoint { get; set; }
    }
}