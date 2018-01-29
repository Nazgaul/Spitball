using System.ComponentModel;
using Cloudents.Core.Enum;
using Cloudents.Mobile.Filters;

namespace Cloudents.Mobile.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Document and flashcard search object
    /// </summary>
    public class SearchRequest : IPaging
    {
        /// <summary>
        /// Array of sites to search for
        /// </summary>
        public string[] Source { get; set; }
        /// <summary>
        /// User university id
        /// </summary>
        public long? University { get; set; }
        /// <summary>
        /// User courses id - Talk to Ram not implemented
        /// </summary>
        public long[] Course { get; set; }
        /// <summary>
        /// The term array of Ai parse
        /// </summary>
        public string[] Query { get; set; }
        /// <summary>
        /// Page for paging
        /// </summary>
        [Paging]
        public int? Page { get; set; }
        /// <summary>
        /// User sort option
        /// </summary>
        [DefaultValue(0)]
        public SearchRequestSort? Sort { get; set; }
        /// <summary>
        /// Doc type only for document vertical
        /// </summary>
        public string DocType { get; set; }

        /// <summary>
        /// Format of result
        /// </summary>
        [DefaultValue(0)]
        public BingTextFormat Format { get; set; }

       
    }
}