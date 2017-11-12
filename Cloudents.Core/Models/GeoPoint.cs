using System.ComponentModel;

namespace Cloudents.Core.Models
{
    [TypeConverter(typeof(GeoPointConverter))]
    public class GeoPoint
    {
        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public static bool TryParse(string s, out GeoPoint result)
        {
            result = null;

            var parts = s.Split(',');
            if (parts.Length != 2)
            {
                return false;
            }

            if (double.TryParse(parts[0], out var latitude)
                && double.TryParse(parts[1], out var longitude))
            {
                result = new GeoPoint { Longitude = longitude, Latitude = latitude };
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return $"{Longitude},{Longitude}";
        }
    }
}
