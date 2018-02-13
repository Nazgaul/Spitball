using System.ComponentModel;

namespace Cloudents.MobileApi.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Book api search request
    /// </summary>
    public class BookRequest : IPaging
    {
        /// <summary>
        /// The term array of Ai parse
        /// </summary>
        public string[] Term { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Page for paging
        /// </summary>
        [DefaultValue(0)]
        public int? Page { get; set; }
    }
}