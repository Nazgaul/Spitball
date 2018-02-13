using System.Runtime.Serialization;

namespace Cloudents.Core.Models
{
    [DataContract]
    public class Location
    {
        public Location(GeoPoint point, Address address, string ip)
        {
            Point = point;
            Address = address;
            Ip = ip;
        }

        protected Location()
        {
        }

        /// <summary>
        /// Location of user
        /// </summary>
        [DataMember(Order = 6)]
        public GeoPoint Point { get; set; }

        [DataMember(Order = 1)]
        public Address Address { get; set; }

        [DataMember(Order = 7)]
        public string Ip { get; set; }
    }
}
