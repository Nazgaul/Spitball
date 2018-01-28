using System.Runtime.Serialization;
using Microsoft.Spatial;

namespace Cloudents.Core.Models
{
    [DataContract]
    public class Location
    {
        [DataMember(Order = 1)]
        public double Longitude { get; set; }
        [DataMember(Order = 2)]
        public double Latitude { get; set; }
        [DataMember(Order = 3)]
        public string City { get; set; }

        [DataMember(Order = 4)]
        public string RegionCode { get; set; }
        [DataMember(Order = 5)]
        public string CountryCode { get; set; }

        public static GeographyPoint ToPoint(Location point)
        {
            if (point == null)
            {
                return null;
            }

            return GeographyPoint.Create(point.Latitude, point.Longitude);
        }
    }
}
