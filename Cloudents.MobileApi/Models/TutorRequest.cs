using System.ComponentModel;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions.Models;

namespace Cloudents.MobileApi.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Tutor request object
    /// </summary>
    public class TutorRequest : IPaging
    {
        /// <summary>
        /// The term array of Ai parse
        /// </summary>
        public string[] Term { get; set; }
        /// <summary>
        /// The filter option
        /// </summary>
        [DefaultValue(0)]
        public TutorRequestFilter[] Filter { get; set; }
        /// <summary>
        /// The sort option
        /// </summary>
        [DefaultValue(0)]
        public TutorRequestSort? Sort { get; set; }
        /// <summary>
        /// The user location
        /// </summary>
        public GeoPoint Location { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Page for paging
        /// </summary>
        [DefaultValue(0)]
        public int? Page { get; set; }
    }
}