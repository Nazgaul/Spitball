using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    /// <summary>
    /// Purchase request model
    /// </summary>
    public class PurchaseRequest
    {
        /// <summary>
        /// The term array of Ai parse
        /// </summary>
        public string[] Term { get; set; }
        /// <summary>
        /// The filter to the request
        /// </summary>
        [DefaultValue(0)]
        public PlacesRequestFilter? Filter { get; set; }
        /// <summary>
        /// The location of the user
        /// </summary>
        [Required]
        public GeographicCoordinate Location { get; set; }

    }
}