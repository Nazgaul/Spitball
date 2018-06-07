using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Web.Models
{
    [DataContract]
    public class GeographicCoordinate
    {
        protected bool Equals(GeographicCoordinate other)
        {
            return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GeographicCoordinate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Latitude.GetHashCode() * 397) ^ Longitude.GetHashCode();
            }
        }

        [Range(-90, 90), DataMember(Order = 1)]
        public float? Latitude { get;  set; }

        [Range(-180, 180), DataMember(Order = 2)]
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

        public static bool operator ==(GeographicCoordinate obj1, GeographicCoordinate obj2)
        {
            return Equals(obj1?.Latitude, obj2?.Latitude)
                    && Equals(obj1?.Longitude, obj2?.Longitude);
        }

        public static bool operator !=(GeographicCoordinate obj1, GeographicCoordinate obj2)
        {
            return !(obj1 == obj2);
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
}