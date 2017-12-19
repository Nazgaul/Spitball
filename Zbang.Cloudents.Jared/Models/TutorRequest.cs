using System.ComponentModel;
using System.Web.Http;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Zbang.Cloudents.Jared.Models
{
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
        /// <summary>
        /// Page for paging
        /// </summary>
        [DefaultValue(0)]
        public int? Page { get; set; }


    }
}