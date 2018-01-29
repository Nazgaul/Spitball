using System.Runtime.Serialization;
using Microsoft.Spatial;

namespace Cloudents.Core.Models
{
    [DataContract]
    public class Location
    {
        //[DataMember(Order = 1)]
        //public double Longitude { get; set; }
        //[DataMember(Order = 2)]
        //public double Latitude { get; set; }

        [DataMember(Order = 6)]
        public GeoPoint Point { get; set; }

        [DataMember(Order = 3)]
        public string City { get; set; }

        [DataMember(Order = 4)]
        public string RegionCode { get; set; }
        [DataMember(Order = 5)]
        public string CountryCode { get; set; }

        public static GeographyPoint ToPoint(Location point)
        {
            if (point?.Point == null)
            {
                return null;
            }

            return GeographyPoint.Create(point.Point.Latitude, point.Point.Longitude);
        }
    }



    [DataContract]
    public class GeoPoint
    {
        //protected bool Equals(GeoPoint other)
        //{
        //    return Longitude.Equals(other.Longitude) && Latitude.Equals(other.Latitude);
        //}

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            var other = (GeoPoint) obj;
           return Longitude.Equals(other.Longitude) && Latitude.Equals(other.Latitude);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Longitude.GetHashCode() * 11) ^ Latitude.GetHashCode();
            }
        }

        [DataMember(Order = 1)]
        public double Longitude { get; set; }
        [DataMember(Order = 2)]
        public double Latitude { get; set; }


        public static bool operator ==(GeoPoint obj1, GeoPoint obj2)
        {
            //if (obj1 == null && obj2 == null)
            //{
            //    return true;
            //}
            if (object.ReferenceEquals(obj1, null))
            {
                return object.ReferenceEquals(obj2, null);
            }

            return obj1.Equals(obj2);
            //if (obj1 != null && obj2 != null)
            //{
            //    return obj1.Latitude == obj2.Latitude && obj1.Longitude == obj2.Longitude;
            //}

            //return false;
        }

        public static bool operator !=(GeoPoint obj1, GeoPoint obj2)
        {
            return !(obj1 == obj2);
        }
    }
}
