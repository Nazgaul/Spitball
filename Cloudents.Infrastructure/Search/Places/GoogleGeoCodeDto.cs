namespace Cloudents.Infrastructure.Search.Places
{
    internal class GoogleGeoCodeDto
    {
        public Result[] Results { get; set; }
        public string Status { get; set; }

        public class Result
        {
            public Address[] AddressComponents { get; set; }
            public string FormattedAddress { get; set; }
            public Geometry Geometry { get; set; }
            public string PlaceId { get; set; }
            public string[] Types { get; set; }
        }

        public class Geometry
        {
            public Bounds Bounds { get; set; }
            public Location Location { get; set; }
            public string LocationType { get; set; }
            public Viewport Viewport { get; set; }
        }

        public class Bounds
        {
            public Northeast Northeast { get; set; }
            public Southwest Southwest { get; set; }
        }

        public class Northeast
        {
            public float Lat { get; set; }
            public float Lng { get; set; }
        }

        public class Southwest
        {
            public float Lat { get; set; }
            public float Lng { get; set; }
        }

        public class Location
        {
            public float Lat { get; set; }
            public float Lng { get; set; }
        }

        public class Viewport
        {
            public Northeast1 Northeast { get; set; }
            public Southwest1 Southwest { get; set; }
        }

        public class Northeast1
        {
            public float Lat { get; set; }
            public float Lng { get; set; }
        }

        public class Southwest1
        {
            public float Lat { get; set; }
            public float Lng { get; set; }
        }

        public class Address
        {
            public string LongName { get; set; }
            public string ShortName { get; set; }
            public string[] Types { get; set; }
        }
    }
}
