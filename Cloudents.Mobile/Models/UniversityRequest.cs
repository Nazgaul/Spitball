using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Models;

namespace Cloudents.Mobile.Models
{
    /// <summary>
    /// University request object
    /// </summary>
    public class UniversityRequest
    {
        /// <summary>
        /// the user input
        /// </summary>
        [StringLength(int.MaxValue, MinimumLength = 3)]
        public string Term { get; set; }
        /// <summary>
        /// the user location
        /// </summary>
        public GeoPoint Location { get; set; }
    }
}