
using Microsoft.Spatial;

namespace Cloudents.Core.Models
{
    //[TypeConverter(typeof(GeoPointConverter))]
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

        //public static bool TryParse(string s, out GeoPoint result)
        //{
        //    result = null;

        //    var parts = s.Split(',');
        //    if (parts.Length != 2)
        //    {
        //        return false;
        //    }

        //    if (double.TryParse(parts[0], out var latitude)
        //        && double.TryParse(parts[1], out var longitude))
        //    {
        //        result = new GeoPoint { Longitude = longitude, Latitude = latitude };
        //        return true;
        //    }
        //    return false;
        //}

        public override string ToString()
        {
            return $"{Latitude},{Longitude}";
        }
    }
}
