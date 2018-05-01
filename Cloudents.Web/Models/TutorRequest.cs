using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Tutor request object
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class TutorRequest : IPaging
    {
        /// <summary>
        /// The term array of Ai parse
        /// </summary>
        public string[] Term { get; set; }
        /// <summary>
        /// The filter option
        /// </summary>
        public TutorRequestFilter[] Filter { get; set; }
        /// <summary>
        /// The sort option
        /// </summary>
        [DefaultValue(0)]
        public TutorRequestSort? Sort { get; set; }
        /// <summary>
        /// The user location
        /// </summary>
        public GeographicCoordinate Location { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Page for paging
        /// </summary>
        [DefaultValue(0)]
        public int? Page { get; set; }
    }
}