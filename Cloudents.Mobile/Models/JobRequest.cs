using System.ComponentModel;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Zbang.Cloudents.Jared.Models
{
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
        /// Filter request
        /// </summary>
        [DefaultValue(0)]
        public JobRequestFilter? Filter { get; set; }
        /// <summary>
        /// Sort Request
        /// </summary>
        [DefaultValue(0)]
        public JobRequestSort? Sort { get; set; }
        /// <summary>
        /// Location of user
        /// </summary>
        public GeoPoint Location { get; set; }
        /// <summary>
        /// Facet filter got from the api
        /// </summary>
        public string[] Facet { get; set; }
        /// <summary>
        /// Page of paging
        /// </summary>
        [DefaultValue(0)]
        public int? Page { get; set; }
    }
}