using System;

namespace Zbang.Zbox.Domain
{
    public class UserLocationActivity
    {
        protected UserLocationActivity()
        {
            
        }
        public UserLocationActivity(Guid id, User user, string domain, double? latitude, double? longitude, string zipCode, string region, string isp, string city, string country, string countryAbbreviation, string browserUserAgent)
        {
            Id = id;
            CreationTime = DateTime.UtcNow;
            User = user;
            Domain = domain;
            Latitude = latitude;
            Longitude = longitude;
            ZipCode = zipCode;
            Region = region;
            ISP = isp;
            City = city;
            Country = country;
            CountryAbbreviation = countryAbbreviation;
            BrowserUserAgent = browserUserAgent;
        }

        public Guid Id { get; private set; }
        public DateTime CreationTime { get; private set; }
        public User User { get; private set; }
        public string Domain { get; private set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public string ZipCode { get; private set; }
        public string Region { get; private set; }
        public string ISP { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public string CountryAbbreviation { get; private set; }

        public string BrowserUserAgent { get; private set; }
    }
}
