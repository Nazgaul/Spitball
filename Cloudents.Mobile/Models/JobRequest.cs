using System.ComponentModel;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Mobile.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Job request api model
    /// </summary>
    public class JobRequest : IPaging
    {
        /// <summary>
        /// The term array of Ai parse
        /// </summary>
        public string[] Term { get; set; }
        /// <summary>
        /// Sort Request
        /// </summary>
        [DefaultValue(0)]
        public JobRequestSort? Sort { get; set; }
        /// <summary>
        /// Location of user
        /// </summary>
        public Location Location { get; set; }
        /// <summary>
        /// Facet filter got from the api
        /// </summary>
        public string[] Facet { get; set; }

        /// <summary>
        /// Should highlight the response with <!--b-->
        /// </summary>
        public bool Highlight { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Page of paging
        /// </summary>
        [DefaultValue(0)]
        public int? Page { get; set; }
    }
}