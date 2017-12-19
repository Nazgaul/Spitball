using System.ComponentModel;

namespace Zbang.Cloudents.Jared.Models
{
    /// <summary>
    /// Ask Question search object
    /// </summary>
    public class AskRequest : IPaging
    {
        /// <summary>
        /// The user text to parse
        /// </summary>
        public string UserText { get; set; }
        /// <summary>
        /// Page for paging
        /// </summary>
        [DefaultValue(0)]
        public int? Page { get; set; }
        /// <summary>
        /// The term array of Ai parse
        /// </summary>
        public string[] Term { get; set; }
    }
}