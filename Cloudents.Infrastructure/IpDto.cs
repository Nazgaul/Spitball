namespace Cloudents.Infrastructure
{
    public class IpDto
    {
        /*    "ip": "72.229.28.185",
    "country_code": "US",
    "country_name": "United States",
    "region_code": "NY",
    "region_name": "New York",
    "city": "New York",
    "zip_code": "10036",
    "time_zone": "America/New_York",
    "latitude": 40.7605,
    "longitude": -73.9933,
    "metro_code": 501*/
        public string Ip { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string TimeZone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string MetroCode { get; set; }


        //public Location ConvertToPoint()
        //{
        //    return new Location(Longitude, Latitude, $"{City},{RegionCode}");
        //}
    }
}
