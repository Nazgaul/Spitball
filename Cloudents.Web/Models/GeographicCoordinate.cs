using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Web.Models
{
   
    public class GeographicCoordinate
    {
        [Range(-90, 90)]
        public float? Latitude { get;  set; }

        [Range(-180, 180)]
        public float? Longitude { get; set; }


        [CanBeNull]
        public GeoPoint ToGeoPoint()
        {
            if (Latitude.HasValue && Longitude.HasValue)
            {
                return new GeoPoint(Longitude.Value, Latitude.Value);
            }

            return null;
        }

        public static GeographicCoordinate FromPoint(GeoPoint point)
        {
            return new GeographicCoordinate()
            {
                Latitude = point.Latitude,
                Longitude = point.Longitude
            };
        }
    }

    public class Location
    {
        public GeographicCoordinate Point { get; set; }

        internal Address Address { get; set; }
        internal string Ip { get; set; }

        public Core.Models.Location ToLocation()
        {
            return new Core.Models.Location(Point.ToGeoPoint(),Address,Ip);
        }
    }
}