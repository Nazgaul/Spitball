using System.Runtime.Serialization;
using Cloudents.Core.Models;

namespace Cloudents.Web.Models
{
    [DataContract]
    public class LocationQuery
    {
        [DataMember(Order = 1)]
        public GeographicCoordinate Point { get; set; }

        [DataMember(Order = 2)]
        internal Address Address { get; set; }
        [DataMember(Order = 3)]
        internal string Ip { get; set; }

        public Location ToLocation()
        {
            return new Location(Point.ToGeoPoint(),Address,Ip);
        }
    }
}