using System.ComponentModel;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Mobile.Models
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
        public GeoPoint Location { get; set; }
    }
}