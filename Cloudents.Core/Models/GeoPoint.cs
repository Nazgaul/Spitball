using Microsoft.Spatial;

namespace Cloudents.Core.Models
{
    public class GeoPoint
    {
        public GeoPoint()
        {
        }

        public GeoPoint(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public static GeographyPoint ToPoint(GeoPoint point)
        {
            if (point == null)
            {
                return null;
            }

            return GeographyPoint.Create(point.Latitude, point.Longitude);
        }

        public override string ToString()
        {
            return $"{Latitude},{Longitude}";
        }
    }
}
