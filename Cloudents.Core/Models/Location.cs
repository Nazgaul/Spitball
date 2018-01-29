using System.Runtime.Serialization;
using Microsoft.Spatial;

namespace Cloudents.Core.Models
{
    [DataContract]
    public class Location
    {
        /// <summary>
        /// Location of user
        /// </summary>
        [DataMember(Order = 6)]
        public GeoPoint Point { get; set; }

        /// <summary>
        /// City of user - for internal purpose
        /// </summary>
        [DataMember(Order = 3)]
        public string City { get; set; }

        /// <summary>
        /// Region of user - for internal purpose
        /// </summary>
        [DataMember(Order = 4)]
        public string RegionCode { get; set; }
        /// <summary>
        /// Country of user - for internal purpose
        /// </summary>
        [DataMember(Order = 5)]
        public string CountryCode { get; set; }

        public static GeographyPoint ToPoint(Location point)
        {
            if (point?.Point == null)
            {
                return null;
            }

            return GeographyPoint.Create(point.Point.Latitude, point.Point.Longitude);
        }
    }
}
