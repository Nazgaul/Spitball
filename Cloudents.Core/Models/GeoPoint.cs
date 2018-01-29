using System.Runtime.Serialization;

namespace Cloudents.Core.Models
{
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
            if (obj.GetType() != GetType()) return false;

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

        /// <summary>
        /// Longitude of user
        /// </summary>
        [DataMember(Order = 1)]
        public double Longitude { get; set; }
        /// <summary>
        /// Latitude of user
        /// </summary>
        [DataMember(Order = 2)]
        public double Latitude { get; set; }


        public static bool operator ==(GeoPoint obj1, GeoPoint obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return ReferenceEquals(obj2, null);
            }

            return obj1.Equals(obj2);
           
        }

        public static bool operator !=(GeoPoint obj1, GeoPoint obj2)
        {
            return !(obj1 == obj2);
        }
    }
}