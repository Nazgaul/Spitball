using System.Runtime.Serialization;

namespace Cloudents.Core.Models
{
    [DataContract]
    public class Location
    {
        public Location(
            string countryCode,
            string? callingCode
            )
        {
            CountryCode = countryCode;
            CallingCode = callingCode;
        }

        [DataMember(Order = 5)]
        public string CountryCode { get; set; }

        [DataMember(Order = 4)]
        public string? CallingCode { get; set; }
    }
}
