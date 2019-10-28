using System.Runtime.Serialization;

namespace Cloudents.Core.Models
{
    [DataContract]
    public class Location
    {
        public Location(
           //GeoPoint point,
           //  string ip,
            string countryCode,
            string callingCode
            )
        {
            // Point = point;
            CountryCode = countryCode;
            //   Ip = ip;
            CallingCode = callingCode;
        }

        /// <summary>
        /// Location of user
        /// </summary>
        //[DataMember(Order = 3)]
        //public GeoPoint Point { get; set; }



        [DataMember(Order = 5)]
        public string CountryCode { get; set; }

        //[DataMember(Order = 2)]
        //public string Ip { get; set; }

        [DataMember(Order = 4)]
        public string CallingCode { get; set; }
    }
}
