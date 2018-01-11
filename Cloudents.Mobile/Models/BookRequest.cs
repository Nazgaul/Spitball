using System.ComponentModel;

namespace Zbang.Cloudents.Jared.Models
{
    /// <summary>
    /// Book api search request
    /// </summary>
    public class BookRequest : IPaging
    {
        /// <summary>
        /// The term array of Ai parse
        /// </summary>
        public string[] Term { get; set; }
        /// <summary>
        /// Page for paging
        /// </summary>
        [DefaultValue(0)]
        public int? Page { get; set; }
        /// <summary>
        /// size of width of image size
        /// </summary>
        [DefaultValue(150)]
        public int? Thumbnail { get; set; }
        
    }
}