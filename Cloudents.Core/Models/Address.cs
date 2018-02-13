using System.Runtime.Serialization;

namespace Cloudents.Core.Models
{
    public class Address
    {
        public Address(string city, string regionCode, string countryCode)
        {
            City = city;
            RegionCode = regionCode;
            CountryCode = countryCode;
        }

        protected Address()
        {
            
        }

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
    }
}