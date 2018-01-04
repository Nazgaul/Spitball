using Cloudents.Core.Models;

namespace Zbang.Cloudents.Jared.Models
{
    /// <summary>
    /// University request object
    /// </summary>
    public class UniversityRequest
    {
        /// <summary>
        /// the user input
        /// </summary>
        public string Term { get; set; }
        /// <summary>
        /// the user location
        /// </summary>
        public GeoPoint Location { get; set; }
    }
}