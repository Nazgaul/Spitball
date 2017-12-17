using System.ComponentModel;
using Cloudents.Core.Enum;

namespace Zbang.Cloudents.Jared.Models
{
    /// <summary>
    /// Document and flashcard search object
    /// </summary>
    public class SearchRequest
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
        public int? Page { get; set; }
        /// <summary>
        /// User sort option
        /// </summary>
        [DefaultValue(0)]
        public SearchCseRequestSort? Sort { get; set; }
    }
}