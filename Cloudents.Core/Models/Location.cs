using System.Runtime.Serialization;

namespace Cloudents.Core.Models
{
    [DataContract]
    public class Location
    {
        public Location(GeoPoint point, Address address, string ip, string callingCode)
        {
            Point = point;
            Address = address;
            Ip = ip;
            CallingCode = callingCode;
        }

        /// <summary>
        /// Location of user
        /// </summary>
        [DataMember(Order = 3)]
        public GeoPoint Point { get; set; }

        [DataMember(Order = 1)]
        public Address Address { get; set; }

        [DataMember(Order = 2)]
        public string Ip { get; set; }

        [DataMember(Order = 4)]
        public string CallingCode { get; set; }
    }
}
