using System.ComponentModel;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Web.Extensions.Models
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
        [CanBeNull]
        [ItemCanBeNull]
        public string[] Term { get; set; }
        /// <summary>
        /// Sort Request
        /// </summary>
        [DefaultValue(0)]
        [CanBeNull]
        public JobRequestSort? Sort { get; set; }
        /// <summary>
        /// Location of user
        /// </summary>
        [CanBeNull]
        public Location Location { get; set; }
        /// <summary>
        /// Facet filter got from the api
        /// </summary>
        [CanBeNull]
        [ItemCanBeNull]
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
        [CanBeNull]
        public int? Page { get; set; }
    }
}